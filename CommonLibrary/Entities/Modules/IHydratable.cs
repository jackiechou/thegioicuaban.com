using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CommonLibrary.Entities.Modules
{
    public interface IHydratable
    {
        int KeyID { get; set; }
        void Fill(IDataReader dr);
    }
}
