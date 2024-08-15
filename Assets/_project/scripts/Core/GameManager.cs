using _project.scripts.Core.GameFlow;
using _project.scripts.Characters;
using _project.scripts.Core.GameFlow.States;
using _project.scripts.Input;
using _project.scripts.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _project.scripts.Core
{
    
    public class GameManager : NetworkBehaviour
    {
        public static int MaxAgent => 1;

        [field: SerializeField] public InputReader Controls { get; private set; }
        [SerializeField] private GameObject agentPrefab;
        [SerializeField] public bool forceGameplay;

        public static GameManager Instance;
        public static GameStateMachine StateMachine { get; private set; }


        private string _partyCode;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(this);
            // get the loaded scene named Gameplay, bc the first scene is the bootstrapper
            forceGameplay = SceneManager.GetSceneAt(1).name == "Gameplay"; 
        }

        private void Start()
        {
            if (!forceGameplay)
            {
                StateMachine = new GameStateMachine(new MainMenuState(), Controls);
            }
        }

        public override void OnNetworkSpawn()
        {
            if(!IsHost) return;
            
            if (forceGameplay)
            {
                StateMachine = new GameStateMachine(new GameplayInitializeState(forceGameplay), Controls);
            }
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        public Agent InstantiateAgents(ulong clientId, int i)
        {
            GameObject agent = Instantiate(agentPrefab);
            agent.name = $"agent {i} p:{clientId}";
            agent.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            return agent.GetComponent<Agent>();
        }

        /// <summary>
        /// The game over menu is open, it's determine the winner or looser
        /// </summary>
        /// <param name="looser">The one who failed the mission</param>
        public void PlayerEndOfGame(ulong looser)
        {
            // Here because of network needs (to run on each client) ClientRCP
            StateMachine.QueueNextState(new OnGameOverState(NetworkManager.Singleton.LocalClientId == looser));
        }

        public void SetPartyCode(string code)
        {
            _partyCode = code;
        }

        public void InitPreparationUI()
        {
            var prep = FindObjectOfType<PreparationUI>();
            if (prep == null) return;
            
            prep.Init(_partyCode);
        }
    }
}