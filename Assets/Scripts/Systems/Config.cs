using System;

namespace Damath.Systems.Configuration
{
    [Serializable]
    public class EOSConfig
    {
        public string ProductId;
        public string SandboxId;
        public string DeploymentId;
        public string ClientId;
        public string ClientSecret;
    }
    
    [Serializable]
    public class Config
    {
        public EOSConfig EOSConfig;
    }
}