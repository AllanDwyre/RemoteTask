using _project.scripts.Core.HealthSystem;

namespace _project.scripts.commands
{
    public class HealCommand : ICommand
    {
        public HealthComponent Receiver { get; private set; }

        public HealCommand(HealthComponent receiver)
        {
            Receiver = receiver;
        }

        public void Execute()
        {
            // TODO : how can I do this until healing is finished, but also without destroy the command principle
            Receiver.HealUntilMax(25, 2);
        }
    }
}