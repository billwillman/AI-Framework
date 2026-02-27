using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TreeDesigner.Editor;

[CustomEditor(typeof(AbilityRunnerComponent), true)]
public class AbilityRunnerDrawer : Editor
{
    VisualElement root;
    AbilityRunnerComponent abilityRunnerComponent;
    Foldout activeTagsFoldout;
    Foldout blockAbilitiesWithTagFoldout;
    Foldout canBufferAbilitiesTagFoldout;

    Foldout activeAbilitiesFoldout;
    BiDictionary<Ability, VisualElement> activeAbilityMap;

    Foldout bufferedAbilitiesFoldout;

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        abilityRunnerComponent = (AbilityRunnerComponent)target;

        activeTagsFoldout = new Foldout();
        activeTagsFoldout.text = "ActiveTags";
        root.Add(activeTagsFoldout);

        blockAbilitiesWithTagFoldout = new Foldout();
        blockAbilitiesWithTagFoldout.text = "BlockAbilitiesWithTag";
        root.Add(blockAbilitiesWithTagFoldout);

        canBufferAbilitiesTagFoldout = new Foldout();
        canBufferAbilitiesTagFoldout.text = "CanBufferAbilitiesTag";
        root.Add(canBufferAbilitiesTagFoldout);

        activeAbilitiesFoldout = new Foldout();
        activeAbilitiesFoldout.text = "ActiveAbilities";
        root.Add(activeAbilitiesFoldout);
        activeAbilityMap = new BiDictionary<Ability, VisualElement>();

        bufferedAbilitiesFoldout = new Foldout();
        bufferedAbilitiesFoldout.text = "BufferedAbilities";
        root.Add(bufferedAbilitiesFoldout);

        EditorApplication.update += Update;
        return root;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }

    void Update()
    {
        if (abilityRunnerComponent.AbilityRunnerOwner != null)
        {
            activeTagsFoldout.Clear();
            foreach (var tag in abilityRunnerComponent.AbilityRunner.ActiveTags)
            {
                Label label = new Label(tag);
                activeTagsFoldout.Add(label);
            }

            blockAbilitiesWithTagFoldout.Clear();
            foreach (var tag in abilityRunnerComponent.AbilityRunner.BlockAbilitiesWithTag)
            {
                Label label = new Label(tag);
                blockAbilitiesWithTagFoldout.Add(label);
            }

            canBufferAbilitiesTagFoldout.Clear();
            foreach (var tag in abilityRunnerComponent.AbilityRunner.CanBufferAbilitiesTag)
            {
                Label label = new Label(tag);
                canBufferAbilitiesTagFoldout.Add(label);
            }

            foreach (var ability in abilityRunnerComponent.AbilityRunner.Abilities)
            {
                if (ability.Active)
                {
                    if (!activeAbilityMap.ContainsKey(ability))
                    {
                        VisualElement abilityElement = new VisualElement();
                        abilityElement.style.flexDirection = FlexDirection.Row;

                        ObjectField objectField = new ObjectField();
                        objectField.value = ability;
                        objectField.SetEnabled(false);
                        abilityElement.Add(objectField);

                        Button button = new Button();
                        button.text = "Open";
                        button.clicked += () => TreeWindowUtility.OpenTree(ability);
                        abilityElement.Add(button);

                        activeAbilitiesFoldout.Add(abilityElement);
                        activeAbilityMap.Add(ability, abilityElement);
                    }
                }
                else
                {
                    if (activeAbilityMap.ContainsKey(ability))
                    {
                        activeAbilitiesFoldout.Remove(activeAbilityMap[ability]);
                        activeAbilityMap.Remove(ability);
                    }
                }
            }

            bufferedAbilitiesFoldout.Clear();
            foreach (var ability in abilityRunnerComponent.AbilityRunner.BufferedAbilities)
            {
                Label label = new Label(ability.name);
                bufferedAbilitiesFoldout.Add(label);
            }
        }
    }
}