using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Option
{
    public class DatabaseOptionBuilder
    {
        DatabaseOption _option;

        public DatabaseOptionBuilder(DatabaseOption option)
        {
            _option = option;
        }

        public DatabaseOption Option => _option;

        public DatabaseOptionBuilder WaitForSync(bool value = true)
        {
            _option.WaitForSync = value;
            return this;
        }

        public DatabaseOptionBuilder ThrowForServerErrors(bool value = true)
        {
            _option.ThrowForServerErrors = true;
            return this;
        }

        public DatabaseOptionBuilder DisableChangeTracking(bool value = true)
        {
            _option.DisableChangeTracking = true;
            return this;
        }
    }
}
