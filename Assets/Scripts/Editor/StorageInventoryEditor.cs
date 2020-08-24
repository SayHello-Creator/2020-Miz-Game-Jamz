﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CustomEditor(typeof(StorageInventory))]
public class StorageInventoryEditor : Editor
{

    StorageInventory inv;

    private int itemID;
    private int itemValue = 1;

    void OnEnable()
    {
        inv = target as StorageInventory;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        AddItemGUI();
        serializedObject.ApplyModifiedProperties();
    }

    void AddItemGUI()                                                                                                       //add a item to the inventory through the inspector
    {
        //inv.SetImportantVariables();
        EditorGUILayout.BeginHorizontal();                                                                                  //starting horizontal GUI elements
        ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");                            //loading the itemdatabase
        string[] items = new string[inventoryItemList.itemList.Count];                                                      //create a string array in length of the itemcount
        for (int i = 1; i < items.Length; i++)                                                                              //go through the item array
        {
            items[i] = inventoryItemList.itemList[i].itemName;                                                              //and paste all names into the array
        }
        itemID = EditorGUILayout.Popup("", itemID, items, EditorStyles.popup);                                              //create a popout with all itemnames in it and save the itemID of it
        itemValue = EditorGUILayout.IntField("", itemValue, GUILayout.Width(40));
        GUI.color = Color.green;                                                                                            //set the color of all following guielements to green
        if (GUILayout.Button("Add Item"))                                                                                   //creating button with name "AddItem"
        {
            inv.AddItemToStorage(itemID, itemValue);
        }

        EditorGUILayout.EndHorizontal();                                                                                    //end the horizontal gui layout
    }
}
