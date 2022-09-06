namespace SymX
{
    /// <summary>
    /// SymStoreTransaction
    /// 
    /// August 1, 2022
    /// 
    /// Defines a symbol store transaction.
    /// </summary>
    public class SymStoreTransaction
    {
        /// <summary>
        /// ID of this symbol store. transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of this symbol server transaction.
        /// </summary>
        public SymStoreTransactionType TransactionType { get; set; }

        /// <summary>
        /// What is being added in this transaction. Only valid if <see cref="TransactionType"/> is <see cref="SymStoreTransactionType.Addition"/>.
        /// </summary>
        public string AdditionType { get; set; }

        /// <summary>
        /// The date of this transaction.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// The time of this transaction.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// The product of this transaction.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The version of the software in this transaction.
        /// </summary>
        public string ProductVersion { get; set; }
        
        /// <summary>
        /// Comments on the transaction.
        /// </summary>
        public string Comments { get; set; }
    }
}
