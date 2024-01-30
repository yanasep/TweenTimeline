using System;
using System.Linq;
using TweenTimeline.Editor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace TweenTimeline
{
    public class SearchDropdownField : DropdownField
    {
        private VisualElement _visualInput;

        private VisualElement visualInput
        {
            get
            {
                if (_visualInput == null)
                {
                    _visualInput = this.Q<VisualElement>(className: "unity-base-popup-field__input");
                }

                return _visualInput;
            }
        }

        public string popupTitle;
        public Action beforeOpenPopup;

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            if (evt == null)
                return;
            bool flag = false;
            if (evt is KeyDownEvent keyDownEvent)
            {
                if (keyDownEvent.keyCode == KeyCode.Space || keyDownEvent.keyCode == KeyCode.KeypadEnter || keyDownEvent.keyCode == KeyCode.Return)
                    flag = true;
            }
            else
            {
                if (evt is MouseDownEvent mouseDownEvent && mouseDownEvent.button == 0
                                                         && this.visualInput.ContainsPoint(
                                                             this.visualInput.WorldToLocal(((MouseEventBase<MouseDownEvent>)evt).mousePosition)))
                    flag = true;
            }

            if (!flag)
                return;
            ShowMenu();
            evt.StopPropagation();
        }

        private void ShowMenu()
        {
            beforeOpenPopup?.Invoke();

            var searchChoices = choices.Select(path => new SearchItem(path)); 

            Action<SearchItem> onItemSelected = item =>
            {
                value = item.Name;
            };

            new SearchButtonAdvancedDropdown(text, searchChoices, onItemSelected, new AdvancedDropdownState()).Show(worldBound);
        }

        /// <summary>
        ///        <para>
        /// Instantiates a DropdownField using the data read from a UXML file.
        /// </para>
        ///      </summary>
        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<SearchDropdownField, UxmlTraits>
        {
        }

        /// <summary>
        ///        <para>
        /// Defines UxmlTraits for the DropdownField.
        /// </para>
        ///      </summary>
        public new class UxmlTraits : DropdownField.UxmlTraits
        {
        }
    }
}
