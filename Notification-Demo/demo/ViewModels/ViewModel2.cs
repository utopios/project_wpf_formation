using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.ViewModels
{
    class ViewModel2
    {
        private string _x_debut, _y_debut, _x_fin, _y_fin;

        public string X_debut { get => _x_debut; set => _x_debut = value; }
        public string Y_debut { get => _y_debut; set => _y_debut = value; }
        public string X_fin { get => _x_fin; set => _x_fin = value; }
        public string Y_fin { get => _y_fin; set => _y_fin = value; }
    }
}
