using KurirServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Intefaces
{
    public interface IDeliveryRepository
    {
        IQueryable<Delivery> ODataGet();
        IEnumerable<Delivery> GetAllDeliveriesAsUser(int UserID = 0);
        IEnumerable<Delivery> GetAllDeliveriesAsCourir(int CourierID = 0);

        IEnumerable<Delivery> GetAllDeliveriesAsDispatcher( int DispatchID = 0);
        IEnumerable<Delivery> GetAllDeliveries();

        IEnumerable<Delivery> GetUncofirmedForDispatcher();
        IEnumerable<Delivery> GetUncofirmedForCourier(int CourierID = 0);
          IEnumerable<Delivery> GetByDate(DateTime from,DateTime to);
        IEnumerable<Delivery> GetByDeliveryType(int deliveryTypeID);
        IEnumerable<Delivery> GetByPaymentType(int paymentTypeID);
        Delivery GetByID(int deliveryID);

    }
}
