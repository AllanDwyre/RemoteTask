using System;
using _project.scripts.commands;
using _project.scripts.Core.CombatSystem;
using _project.scripts.Core.HealthSystem;
using Unity.Netcode;

namespace _project.scripts.Characters
{
    /// <summary>
    /// Is an orchestrator, it only dealt with communication, and orders.
    /// </summary>
    public abstract class CharacterBase : NetworkBehaviour
    {
        private readonly CommandManager _controller = new();
        public abstract string CharacterName { get; protected set; }
        public abstract int Faction { get; protected set; }
        public MovementComponent MovementComponent {get; protected set;}
        public HealthComponent HealthComp {get; protected set;}
        public CombatComponent Combat {get; protected set;}
        protected bool HasPermission => IsOwner || IsOwnedByServer;
        private bool IsIdle { get; set; } = true;

        protected virtual void Awake()
        {
            Combat =  GetComponent<CombatComponent>();
            MovementComponent = GetComponent<MovementComponent>();
            HealthComp = GetComponent<HealthComponent>();
            HealthComp.OnDeath += OnCharacterDeath;
        }

  
        protected void Update()
        {
            if(!IsIdle || _controller.Count == 0) return;
            ExecuteNextCommand();
        }
        
        #region CommandHandling
        protected abstract bool EvaluateCommand(ICommand command);
        public void OnTaskCompleted()
        {
            if(!HasPermission) return;
            
            IsIdle = true;
            ExecuteNextCommand();
        }
        public void ExecuteCommand(ICommand command)
        {
            if(!HasPermission) return;
            
            if (!EvaluateCommand(command)) return;

            IsIdle = false;
            command.Execute();
        }

        private void ExecuteNextCommand()
        {
            var command = _controller.DequeueCommand();
            if (command == null || !EvaluateCommand(command)) return;
            IsIdle = false;
            command.Execute();
        }

        public void AddCommand(ICommand command)
        {
            if(!HasPermission) return;
            _controller.AddCommand(command);
        }

        #endregion
        #region HealthRelated
        protected virtual void OnCharacterDeath()
        {
            MovementComponent.DisableMovement();
            Combat.DisableCombat();
        }
        #endregion

    }
}