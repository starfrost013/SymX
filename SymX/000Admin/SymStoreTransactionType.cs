using System;

namespace SymX
{
    /// <summary>
    /// SymStoreTransactionType
    /// 
    /// August 7, 2022
    /// 
    /// Enumerates symbol server transaction types.
    /// </summary>
    public enum SymStoreTransactionType
    {
        /// <summary>
        /// Addition of a set of symbols to the symbol server.
        /// </summary>
        Add = 0,

        /// <summary>
        /// 'Deletion' of a set of symbols from the symbol server.
        /// It is not deleted, it just adds ".deleted" to the transaction ID
        /// </summary>
        Del = 1,
    }
}
