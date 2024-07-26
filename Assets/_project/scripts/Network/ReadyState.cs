using _project.scripts.UI;
using Unity.Netcode;

namespace _project.scripts.Network
{
    public class ReadyState : NetworkBehaviour
    {
        public NetworkVariable<int> NbOfReadyPeople { get; private set; } = new(writePerm:NetworkVariableWritePermission.Owner);
        
        public override void OnNetworkSpawn()
        {
            if (!IsHost) return;
            
            GetComponent<PreparationUI>().IsReady.OnValueChanged += OnReadyChange;
        }

        private void OnReadyChange(bool prev, bool current)
        {
            NbOfReadyPeople.Value += current ? 1 : -1;
        }
    }
}