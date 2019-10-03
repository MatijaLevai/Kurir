using KurirServer.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer.Helpers
{
    public static class OdataModelBuilder
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<User>("Users");
            builder.EntitySet<Delivery>("Deliveries");
           // builder.EntitySet<DeliveryType>("DeliveryTypes");
            //builder.EntitySet<Role>("Roles");
            //builder.EntitySet<UserRole>("UserRoles");
            //builder.EntitySet<Location>("Locations");
           // builder.EntitySet<PaymentType>("PaymentTypes");
            return builder.GetEdmModel();
        }
    }
}

