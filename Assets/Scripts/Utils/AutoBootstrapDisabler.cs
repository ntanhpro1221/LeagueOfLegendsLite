using Unity.NetCode;

public class AutoBootstrapDisabler : ClientServerBootstrap {
    public override bool Initialize(string defaultWorldName) {
        CreateLocalWorld(defaultWorldName);
        return true;
    }
}
