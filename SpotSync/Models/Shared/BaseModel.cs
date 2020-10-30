using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotSync.Models.Shared
{
    public class BaseModel<T> : BaseModel
    {
        public BaseModel() { }
        public BaseModel(T model, BaseModel baseModel) : base(baseModel)
        {
            PageModel = model;
        }

        public T PageModel { get; set; }
    }

    public class BaseModel
    {
        public BaseModel() { }
        public BaseModel(BaseModel model)
        {
            IsUserInParty = model.IsUserInParty;
            PartyCode = model.PartyCode;
            ErrorMessage = model.ErrorMessage;
        }

        public BaseModel(bool joinedInParty = false, string partyCode = null, string errorMessage = null)
        {
            IsUserInParty = joinedInParty;
            PartyCode = partyCode;
            ErrorMessage = errorMessage;
        }
        public bool IsUserInParty { get; set; }
        public string PartyUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PartyCode))
                    return $"/party?partyCode={PartyCode}";
                else
                    return null;
            }
        }

        public string PartyCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
