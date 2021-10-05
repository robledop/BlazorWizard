using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWizard
{
    public partial class WizardStep
    {
        [Parameter] public Func<Task<bool>> OnNext { get; set; } = () => Task.FromResult(true);

        /// <summary>
        ///     The <see cref="Wizard" /> container
        /// </summary>
        [CascadingParameter]
        protected internal Wizard Parent { get; set; }

        /// <summary>
        ///     The Child Content of the current <see cref="WizardStep" />
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///     The Name of the wizard step
        /// </summary>
        [Parameter]
        public string Name { get; set; }

        /// <summary>
        ///     Determines if the <see cref="WizardStep" /> should be added to the WizardSteps list
        /// </summary>
        [Parameter]
        public bool ShouldBeAdded { get; set; } = true;

        /// <summary>
        ///     Determines the <see cref="WizardStep" />'s position in the WizardSteps list
        /// </summary>
        [Parameter]
        public int Position { get; set; } = 1;

        protected override void OnParametersSet()
        {
            if (ShouldBeAdded)
            {
                Parent.AddStep(this);
            }
            else
            {
                Parent.RemoveStep(this);
            }
        }
    }
}