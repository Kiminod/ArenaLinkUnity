using System;

namespace ArenaLink.ArenaLink.Config
{
    public class ArenaLinkBuilder
    {
        private readonly ArenaLinkOptions _options = new();

        public ArenaLinkBuilder Configure(Action<ArenaLinkOptions> configAction)
        {
            configAction?.Invoke(_options);
            return this;
        }
        
        public ArenaLinkService Build()
        {
            return new ArenaLinkService(_options);
        }
    }
}