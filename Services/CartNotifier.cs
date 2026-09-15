using System;
using System.Collections.Generic;
using System.Text;

namespace E_store.Services
{
    //TODO
    internal class CartNotifier
    {
        public static event Action CartUpdated;

        public static void Notify()
        {
            CartUpdated?.Invoke();
        }

    }
}
