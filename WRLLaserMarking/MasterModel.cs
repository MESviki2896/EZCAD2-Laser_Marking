using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WRLLaserMarking
{
    public class MasterModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
        public string AssetVariable { get; set; }
        public string ImageName { get; set; }
        public byte[] AssetImage { get; set; }

        public static  MasterModel AddModelValues(string ProductCode, string ProductName, string Alias, string Type, string AssetVariable, string ImageName, byte[] AssetImage)
        {
            MasterModel model = new MasterModel();
            model.ProductCode = ProductCode;
            model.ProductName = ProductName;
            model.Alias = Alias;
            model.Type = Type;
            model.AssetVariable = AssetVariable;
            model.ImageName = ImageName;
            model.AssetImage = AssetImage;
            return model;
        }
    }
    

    //Might Require this class Someday
    public class ReturnMaster
    {
        public Type ReturnType;
        public object ReturnData;
    }
}
