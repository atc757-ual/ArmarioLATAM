using ArmarioLATAM.Components.Models;
using System.Collections.Generic;
using System.Linq;

namespace ArmarioLATAM.Components
{
    public class KitSelectionState
    {
        public int KitTypeId { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }

        public void Set(KitType kit)
        {
            KitTypeId = kit.KitTypeId;
            Name = kit.Name;
            Description = kit.Description;
        }

        public void Clear()
        {
            KitTypeId = 0;
            Name = null;
            Description = null;
        }
    }
}
