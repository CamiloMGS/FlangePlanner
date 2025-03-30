using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Preliy.Flange.Planner;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

[CustomPropertyDrawer(typeof(SequenceHandler.RawInstruction))]
public class IntructionDrawerHS : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();

        SerializedProperty instructionTypeProp = property.FindPropertyRelative("instructionType");
        SerializedProperty monoInstructionProp = property.FindPropertyRelative("monoInstruction");
        SerializedProperty gameObjectProp = property.FindPropertyRelative("gameObject");

        EnumField enumField = new EnumField("Instruction: ", SequenceHandler.InstructionType.None)
        {
            bindingPath = instructionTypeProp.propertyPath,
        };

        PropertyField monoInstructionField = new PropertyField(monoInstructionProp, "Mono Instruction")
        {
            bindingPath = monoInstructionProp.propertyPath
        };

        PropertyField gameObjectField = new PropertyField(gameObjectProp, "Game Object")
        {
            bindingPath = gameObjectProp.propertyPath
        };

        enumField.RegisterValueChangedCallback(evt =>
        {
            if (evt.previousValue != evt.newValue)
            {
                Enum previousValue = evt.previousValue;
                Enum newValue = evt.newValue;

                int previousIntValue = Convert.ToInt32(previousValue);
                int newIntValue = Convert.ToInt32(newValue);


                SequenceHandler.OnInstructionChanged?.Invoke(this,
                new SequenceHandler.OnInstructionChangedEventArgs(
                    ExtractIndex(property.propertyPath),
                    previousIntValue
                , newIntValue));
            }
        });

        gameObjectField.RegisterValueChangeCallback(evt =>
        {
            //var value = evt.changedProperty.enumValueIndex;
            //Debug.Log(value);
        });

        container.Add(enumField);
        //container.Add(monoInstructionField);
        //container.Add(gameObjectField);

        return container;
    }


    public static int ExtractIndex(string input)
    {
        int start = input.IndexOf('[');
        int end = input.IndexOf(']', start);

        if (start >= 0 && end > start)
        {
            string number = input.Substring(start + 1, end - start - 1);
            if (int.TryParse(number, out int result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Unable to convert the value to an integer.");
            }
        }
        else
        {
            throw new ArgumentException("The string does not contain a valid index.");
        }
    }

}
