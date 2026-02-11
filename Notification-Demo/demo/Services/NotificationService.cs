using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.Services
{
    class NotificationService
    {
        public event Action<string, string> OnNotification;
        public NotificationService() { }
        public void Notify(string property, string value)
        {
            OnNotification?.Invoke(property, value);
        }
    }
}
