using _project.scripts.commands;

namespace _project.scripts.Characters
{
    public class Enemy : CharacterBase
    {
        public override string CharacterName { get; protected set; } = "Sam The Duck";
        public override int Faction { get; protected set; }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Faction = -1;
            if(!IsServer) Destroy(this);
        }

        

        protected override bool EvaluateCommand(ICommand command)
        {
            return true;
        }
    }
}