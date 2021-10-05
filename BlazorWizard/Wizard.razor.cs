using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWizard
{
    public partial class Wizard
    {
        private bool _isProgressBarVisible;

        /// <summary>
        ///     List of <see cref="WizardStep" /> added to the Wizard
        /// </summary>
        protected internal List<WizardStep> Steps = new();

        [Parameter] public string Id { get; set; }

        /// <summary>
        ///     The ChildContent container for <see cref="WizardStep" />
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///     The Active <see cref="WizardStep" />
        /// </summary>
        [Parameter]
        public WizardStep ActiveStep { get; set; }

        /// <summary>
        ///     The Index number of the <see cref="ActiveStep" />
        /// </summary>
        [Parameter]
        public int ActiveStepIndex { get; set; }

        [Parameter]
        public bool IsProgressBarVisible
        {
            get => _isProgressBarVisible;
            set
            {
                IsNextButtonDisabled = value;
                _isProgressBarVisible = value;
                StateHasChanged();
            }
        }


        /// <summary>
        ///     Determines whether the Wizard is in the last step
        /// </summary>
        private bool IsLastStep => ActiveStepIndex == Steps.Count - 1;

        /// <summary>
        ///     Determines whether the Wizard is in the step where the form needs to be submitted
        /// </summary>
        private bool IsSubmitStep => ActiveStepIndex == Steps.Count - 2;

        /// <summary>
        ///     Determines whether the Wizard is in a regular step
        /// </summary>
        private bool IsRegularStep => ActiveStepIndex < Steps.Count - 2;

        [Parameter] public EventCallback OnSubmit { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
        [Parameter] public bool IsNextButtonDisabled { get; set; }

        /// <summary>
        ///     Sets the <see cref="ActiveStep" /> to the previous Index
        /// </summary>
        protected internal void GoBack()
        {
            if (ActiveStepIndex > 0)
            {
                SetActive(Steps.OrderBy(s => s.Position).ToList()[ActiveStepIndex - 1]);
            }
        }

        /// <summary>
        ///     Sets the <see cref="ActiveStep" /> to the next Index
        /// </summary>
        protected internal async Task GoNext()
        {
            if (ActiveStep.OnNext is null)
            {
                return;
            }

            if (await ActiveStep.OnNext())
            {
                if (IsRegularStep)
                {
                    SetNextStepActive();
                }
                else if (IsSubmitStep)
                {
                    SetNextStepActive();
                    await Submit();
                }
                else if (IsLastStep)
                {
                    await Close();
                }
            }
        }

        private void SetNextStepActive()
        {
            SetActive(Steps.OrderBy(s => s.Position).ToList()[Steps.IndexOf(ActiveStep) + 1]);
        }

        /// <summary>
        ///     Populates the <see cref="ActiveStep" /> then sets the passed in <see cref="WizardStep" /> instance as the active
        ///     step
        /// </summary>
        /// <param name="step">The WizardStep</param>
        protected void SetActive(WizardStep step)
        {
            ActiveStep = step ?? throw new ArgumentNullException(nameof(step));

            ActiveStepIndex = StepsIndex(step);
        }


        /// <summary>
        ///     Retrieves the index of the current <see cref="WizardStep" /> in the Step List
        /// </summary>
        /// <param name="step">The WizardStep</param>
        /// <returns></returns>
        public int StepsIndex(WizardStep step)
        {
            return StepsIndexInternal(step);
        }

        protected int StepsIndexInternal(WizardStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            return Steps.IndexOf(step);
        }

        /// <summary>
        ///     Adds a <see cref="WizardStep" /> to the WizardSteps list
        /// </summary>
        /// <param name="step"></param>
        protected internal void AddStep(WizardStep step)
        {
            if (Steps.Contains(step))
            {
                return;
            }

            Steps.Add(step);
        }

        /// <summary>
        ///     Remove a <see cref="WizardStep" /> from the WizardSteps list
        /// </summary>
        /// <param name="step"></param>
        protected internal void RemoveStep(WizardStep step)
        {
            if (Steps.Contains(step))
            {
                Steps.Remove(step);
            }
        }

        protected internal async Task Close()
        {
            await OnClose.InvokeAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            SetActive(Steps[0]);
            StateHasChanged();
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }

        private async Task Submit()
        {
            await OnSubmit.InvokeAsync();
        }
    }
}