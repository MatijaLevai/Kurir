using KurirServer.Entities;
using KurirServer.Intefaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private KurirDbContext context;
        public DeliveryRepository(KurirDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Delivery> ODataGet()
        {
            return context.Deliveries;
        }
        public IEnumerable<Delivery> GetAllDeliveries()
        {
           return context.Deliveries.ToList();
        }

        public IEnumerable<Delivery> GetAllDeliveriesAsCourir(int CourierID = 0)
        {
            if (CourierID <= 0)
            {
                return null;
            }
            else
            {
                return context.Deliveries.ToList().Where(d => d.CourierID == CourierID); ;
            }
        }

        public IEnumerable<Delivery> GetAllDeliveriesAsDispatcher(int DispatchID = 0)
        {
            if (DispatchID <= 0)
            {
                return null;
            }
            else
            {
                IEnumerable<Delivery> returnD = context.Deliveries.ToList().Where(d => d.DispatcherID == DispatchID);
                foreach (var item in returnD)
                {
                    item.StartAddress = context.Addresses.Where(a => a.FullAddressID == item.StartAddressID).First();
                    item.EndAddress = context.Addresses.Where(a => a.FullAddressID == item.EndAddressID).First();
                }
                return returnD;

            }

        }

        public IEnumerable<Delivery> GetAllDeliveriesAsUser(int UserID = 0)
        {
            if (UserID <= 0)
            {
                return null;
            }
            else
            {
                IEnumerable<Delivery> returnD = context.Deliveries.ToList().Where(d => d.UserID == UserID);
                foreach (var item in returnD)
                {
                    item.StartAddress = context.Addresses.Where(a => a.FullAddressID == item.StartAddressID).First();
                    item.EndAddress = context.Addresses.Where(a => a.FullAddressID == item.EndAddressID).First();
                }
                return returnD;

            }

        }
//       
        public IEnumerable<Delivery> GetByDate(DateTime from, DateTime to)
        {
            if (from != null && to != null)
            {
                if (from < to)
                {
                    IQueryable<Delivery> q = context.Deliveries;
                    return q.Where(d => (d.StartTime >= from) && (d.EndTime < to));
                }
                else
                {
                    IQueryable<Delivery> q = context.Deliveries;
                    return q.Where(d => (d.StartTime >= to) && (d.EndTime < from));
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Delivery> GetByDeliveryType(int deliveryTypeID)
        {
            if (deliveryTypeID > 0)
            {
                IQueryable<Delivery> q = context.Deliveries;
                IEnumerable<Delivery> returnD =  q.Where(d => (d.DeliveryTypeID == deliveryTypeID));
                // context.Deliveries.ToList().Where(d => d.DispatcherID == DispatchID);
                foreach (var item in returnD)
                {
                    item.StartAddress = context.Addresses.Where(a => a.FullAddressID == item.StartAddressID).First();
                    item.EndAddress = context.Addresses.Where(a => a.FullAddressID == item.EndAddressID).First();
                }
                return returnD;
            }
            else
            {
                return null;
            }
        }

        public Delivery GetByID(int deliveryID)
        {

            IQueryable<Delivery> query = context.Deliveries.AsQueryable();
            var result = query.Where(d => d.DeliveryID == deliveryID).FirstOrDefault();
            if (result != null)
                return result;
            else return null;

        }

        public IEnumerable<Delivery> GetByPaymentType(int paymentTypeID)
        {
            if (paymentTypeID > 0)
            {
                IQueryable<Delivery> q = context.Deliveries;
                return q.Where(d => (d.PaymentTypeID == paymentTypeID));
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Delivery> GetUncofirmedForDispatcher()
        {
            IEnumerable<Delivery> returnD = context.Deliveries.ToList().Where(d => (d.DispatcherID == 0) && (d.StartTime < d.CreateTime));
            foreach (var item in returnD)
            {
                item.StartAddress = context.Addresses.Where(a => a.FullAddressID == item.StartAddressID).First();
                item.EndAddress = context.Addresses.Where(a => a.FullAddressID == item.EndAddressID).First();
            }
            return returnD;
                    


        }

        
        public IEnumerable<Delivery> GetUncofirmedForCourier(int CourierID = 0)
        {
            if(CourierID <= 0)
            {
                return context.Deliveries.ToList();
            }

            else
            {
                return context.Deliveries.ToList().Where(d => d.CourierID == CourierID); ;
            }
        }

        
        


        /*    public async Task<Delivery> EditDelivery(Delivery newD)
            {
                if (newD != null)
                {

                    var delivery = context.Deliveries.Where(d => d.DeliveryID == newD.DeliveryID).FirstOrDefault();
                    if (delivery != null)
                    {
                        context.Update(newD);
                        var x = await context.SaveChangesAsync();
                        Debug.WriteLine(x);
                        return newD;
                    }
                    return null;
                }
                else return null;
            }
            */
    }
}
