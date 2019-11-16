using System;
using System.Collections.Generic;
using System.Text;

namespace Kurir.Models
{
    public class DeliveryStatusModel
    {
       public int DeliveryStatusID
        {
            get;set;
        }
        public string DeliveryStatusImageSource { get; set; }
        public string Opis { get; set; }
        public void SetOpis()
        {
            {
                switch (DeliveryStatusID)
                {
                    case 1:
                        Opis = "Dispečer odobrio";
                        break;
                    case 2:
                        Opis = "Kurir prihvatio";
                        break;
                    case 3:
                        Opis = "Kurir preuzeo";
                        break;
                    case 4:
                        Opis = "Kurir dostavio";
                        break;
                }
            }
        }
    }
}
