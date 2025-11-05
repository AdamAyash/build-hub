using BuildHubCommon.ConfigurationManager.Base;
using Microsoft.Extensions.Configuration;

namespace BuildHubCommon.ConfigurationManager
{
    /// <summary>
    /// Manager class for shared configurations across the solution
    /// </summary>
    public class SharedConfigurationManager : BaseConfigurationManager
    {
        public SharedConfigurationManager()
        {
        } 

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            var builder = new ConfigurationBuilder();


            _configuration = builder.Build();
        }
    }
}
