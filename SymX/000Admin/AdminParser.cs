using NuCore.Utilities;
using System.IO;

namespace SymX
{
    /// <summary>
    /// AdminParser
    /// 
    /// July 29, 2022
    /// 
    /// Parses a /000admin directory if it exists on the server.
    /// </summary>
    public static class AdminParser
    {
        public static bool Parse000Admin()
        {
            if (Configuration.IsMsdl())
            {
                NCLogging.Log("MSDL is not supported in this mode. MSDL is not a normal symbol server, " +
                    "and uses a custom Microsoft Azure-based setup for better scalability and reliability, " +
                    "as it is used on the daily for new builds of Windows. Any 000admin folder" +
                    "would have been removed when they moved to this setup in 2017.", ConsoleColor.Red, false, false);
                return false;
            }

            string tempFileName = Path.GetTempFileName();
            string historyUrl = $@"{Configuration.SymbolServerUrl}\000Admin\history.txt";
            NCLogging.Log($"Obtaining symstore history. Downloading {historyUrl} to {tempFileName}...");

            // download history.txt
            using (BinaryWriter bw = new BinaryWriter(new FileStream(tempFileName, FileMode.Create)))
            {
                Task<byte[]> historyTextFile = HttpManager.Client.GetByteArrayAsync(historyUrl);

                // wait for it to download
                while (!historyTextFile.IsCompleted) { };

                bw.Write(historyTextFile.Result);
            }

            List<SymStoreTransaction> transactions = ParseHistory(tempFileName);

            foreach (SymStoreTransaction transaction in transactions)
            {
                if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Processing and downloading transaction {transaction.Id}");
                
                if (!DownloadTransaction(transaction))
                {
                    NCLogging.Log("An error occurred downloading transactions.", ConsoleColor.Red);
                    return false; 
                }
            }

            // temp
            return true;
        }

        private static List<SymStoreTransaction> ParseHistory(string historyFileName)
        {
            if (Configuration.Verbosity >= Verbosity.Normal) NCLogging.Log("Reading history...");
            string[] history = File.ReadAllLines(historyFileName);
            List<SymStoreTransaction> transactions = new List<SymStoreTransaction>();

            foreach (string curTransaction in history)
            {
                SymStoreTransaction transaction = new SymStoreTransaction();

                string[] transactionComponents = curTransaction.Split(',');

                string transactionIdString = transactionComponents[0];
                SymStoreTransactionType transactionType = Enum.Parse<SymStoreTransactionType>(transactionComponents[1], true); // ignore case because it's always lowercase

                int transactionId = Convert.ToInt32(transactionIdString);

                transaction.Id = transactionId;
                transaction.TransactionType = transactionType;

                // just in case

                switch (transactionType)
                {
                    case SymStoreTransactionType.Add:
                        // addition transaction
                        if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Transaction ID {transactionId}");

                        string typeAdded = transactionComponents[2];
                        string date = transactionComponents[3];
                        string time = transactionComponents[4];
                        string productName = transactionComponents[5];
                        string productVersion = transactionComponents[6];
                        string comment = transactionComponents[7];

                        if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log("Transaction Information:\n" +
                            $"Type: {typeAdded}\n" +
                            $"Date: {date} {time}\n" +
                            $"Product: {productName}, version {productVersion}\n" +
                            $"Comments: {comment}");

                        transaction.AdditionType = typeAdded;
                        transaction.Date = date;
                        transaction.Time = time;
                        transaction.ProductName = productName;
                        transaction.ProductVersion = productVersion;
                        transaction.Comments = comment;

                        transactions.Add(transaction);
                        continue;
                    case SymStoreTransactionType.Del:
                        string deletedTransactionValue = transactionComponents[2];
                        int deletedTransactionId = Convert.ToInt32(deletedTransactionValue);
                        if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Transaction ID {transactionId} deleting transaction ID {deletedTransactionId}!");

                        transactions.Add(transaction);
                        continue;
                }

            }

            return transactions;
        }

        private static bool DownloadTransaction(SymStoreTransaction transaction)
        {
            // D10 = pad to string
            string transId = transaction.Id.ToString("D10");

            // symsrv just renames folders when you delete a transaction
            // so we try two different URLs
            string baseUrl = $"{Configuration.SymbolServerUrl}/000Admin/{transId}";
            string baseDeletedUrl = $"{Configuration.SymbolServerUrl}/000Admin/{transId}.deleted";

            string historyDownloadUrl = baseUrl;

            // try "normal" url
            HttpRequestMessage requestMsg = new HttpRequestMessage(HttpMethod.Head, baseUrl);

            if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Trying {baseUrl} for transaction information");
            // send sync for now
            HttpResponseMessage response = HttpManager.Client.Send(requestMsg);

            if (!response.IsSuccessStatusCode)
            {
                // try the "deleted" url
                HttpRequestMessage deletedMsg = new HttpRequestMessage(HttpMethod.Head, baseDeletedUrl);

                if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Trying {baseUrl} for transaction information (\"deleted\") transaction");
                // send sync for now
                HttpResponseMessage deletedResponse = HttpManager.Client.Send(deletedMsg);

                if (!deletedResponse.IsSuccessStatusCode)
                {
                    if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Transaction {transaction.Id} failed to download: Likely does not exist. Failed both normal and deleted URL");
                    return false;
                }

                // set if successful
                historyDownloadUrl = baseDeletedUrl;
            }

            if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Downloading {historyDownloadUrl}...");

            Task<byte[]> downloadPassUrl = HttpManager.Client.GetByteArrayAsync(historyDownloadUrl);

            // wait until done
            while (!downloadPassUrl.IsCompleted) { };
            // get a temporary file name
            string tempFileName = Path.GetTempFileName();

            // save file
            byte[] fileBytes = downloadPassUrl.Result;
            File.WriteAllBytes(tempFileName, fileBytes);
            if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log($"Saved to {tempFileName}!");

            string[] fileLines = File.ReadAllLines(tempFileName);

            if (Configuration.Verbosity >= Verbosity.Verbose) NCLogging.Log("Parsing transaction data...");

            List<string> filesToDownload = new List<string>();

            foreach (string line in fileLines)
            {
                string[] splitLine = line.Split(',');

                // split filename and imagesize from the original file name
                string fileNameAndImageSize = splitLine[0];
                string originalFileName = splitLine[1];

                // Strip quote characters
                fileNameAndImageSize = fileNameAndImageSize.Replace("\"", "");
                originalFileName = originalFileName.Replace("\"", "");

                string[] fileNameComponents = fileNameAndImageSize.Split('\\');

                // Build the actual url
                string fileName = fileNameComponents[0];

                string downloadUrl = $"{Configuration.SymbolServerUrl}{fileNameAndImageSize}/{fileName}";

                if (Configuration.Verbosity >= Verbosity.Verbose)
                {
                    NCLogging.Log($"Adding {downloadUrl} to download queue (original file name, if available: {originalFileName})...");
                }

                filesToDownload.Add(downloadUrl);
            }

            // clean up the temp file
            File.Delete(tempFileName);

            //temp
            return FileDownloader.DownloadListOfFiles(filesToDownload);
        }
    }
}
