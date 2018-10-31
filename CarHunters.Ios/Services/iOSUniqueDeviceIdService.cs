using System;
using CarHunters.Core.PlatformAbstractions;
using Foundation;
using Security;

namespace CarHunters.Ios.Services
{
    public class iOSUniqueDeviceIdService : IUniqueDeviceIdService
    {
        string uniqueRecordName = "carHunterDeviceId";

        public string UniqueDeviceId
        {
            get
            {
                return AttemptGetDeviceId();
            }
        }

        string AttemptGetDeviceId()
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Generic = NSData.FromString(uniqueRecordName)
            };

            SecStatusCode securityStatusCode;

            var recordMatch = SecKeyChain.QueryAsRecord(record, out securityStatusCode);

            if (securityStatusCode == SecStatusCode.Success)
            {
                return recordMatch.ValueData.ToString();
            }
            else
            {
                var newRecord = new SecRecord(SecKind.GenericPassword)
                {
                    ValueData = NSData.FromString(Guid.NewGuid().ToString()),
                    Generic = NSData.FromString(uniqueRecordName)
                };
                SecKeyChain.Add(newRecord);
                return newRecord.ValueData.ToString();
            }
        }
    }
}
