using _project.scripts.commands;

namespace _project.scripts.Characters
{
    public class Enemy : CharacterBase
    {
        public override string CharacterName { get; protected set; } = "Sam The Duck";
        public override int Faction { get; protected set; } = -1;
        
        protected override bool EvaluateCommand(ICommand command)
        {
            return true;
        }
    }
}