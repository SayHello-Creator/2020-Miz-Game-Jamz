﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int Damage = 1;
    [SerializeField]
    private int CurrHP = 2;
    [SerializeField]
    private int MaxHP = 2;
    [SerializeField]
    private int Armor = 0;
    [SerializeField]
    private int InventorySize = 5;

    [SerializeField]
    private InventoryUI InventoryUI;

    private Inventory EquippedItems;
    private Inventory Inventory;

    public static Player Instance = null;

    public enum PlayerState
    {
        Moving,
        NotMoving,
        Interacting,
        Attacking,
        TakingDamage,
        Ghosting,
        Healing,
        InInventory
    }

    public PlayerState CurrentState;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Instance = this;
        CurrentState = PlayerState.NotMoving;

        this.Inventory = new Inventory(UseItem);
        this.Inventory.Size = InventorySize;
        this.EquippedItems = new Inventory(null);
        this.EquippedItems.Size = 3;
        this.InventoryUI.SetInventory(this.Inventory);
        this.InventoryUI.SetEquippedItems(this.EquippedItems);

        //Get a component reference to the Player's animator component
        //animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        //When Player object is disabled, store stuff in the GameManager so it can be re-loaded in next level.
    }

    private void Update()
    {
        if (!GameManager.Instance.PlayersTurn || CurrentState == PlayerState.Moving) return;
        CheckForPlayerPause();
    }

    private void UseItem(Item item)
    {
        if(item.Type == Item.ItemType.Consumable)
        {
            ManageCurrHealth(0, item.HealthMod);
            Inventory.RemoveItem(new Item { Type = item.Type, Name = item.Name, Amount = 1, ArmorMod = item.ArmorMod, DamageMod = item.DamageMod, HealthMod = item.HealthMod });
        }
    }

    private void CheckForPlayerPause()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!(CurrentState == PlayerState.InInventory))
            {
                InventoryUI.OpenInventory();
                CurrentState = PlayerState.InInventory;
            }
            else
            {
                InventoryUI.CloseInventory();
                CurrentState = PlayerState.NotMoving;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemInWorld itemWorld = collision.GetComponent<ItemInWorld>();
        if (itemWorld != null && this.Inventory.GetItems().Count < this.Inventory.Size)
        {
            this.Inventory.AddItem(itemWorld.GetItem());
            itemWorld.SelfDestruct();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            collision.gameObject.GetComponent<Interactable>()
                .Interact<Player>(this);
        }
    }

    public void Check()
    {
        //any checks that need done put in here (check should be performed at the start of a turn and end) 

        CheckIfGameOver();

        GameManager.Instance.PlayersTurn = false;
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void ManageCurrHealth(int loss, int gain)
    {
        loss = Mathf.Abs(loss);
        gain = Mathf.Abs(gain);

        if (loss > 0)
        {
            //TODO: animator.SetTrigger("Hit");

            CurrHP -= loss;
            if (CurrHP > MaxHP) CurrHP = MaxHP;
            CheckIfGameOver();
        }
        else
        {
            //TODO: Set the trigger for player get health
            //somthing to do with a health particle effect probably

            CurrHP += gain;
            if (CurrHP > MaxHP) CurrHP = MaxHP;
        }
    }

    public void ManageMaxHealth(int loss, int gain)
    {
        loss = Mathf.Abs(loss);
        gain = Mathf.Abs(gain);

        if (loss > 0)
        {
            MaxHP -= loss;
            if (MaxHP <= 0) MaxHP = 1;
            if (CurrHP > MaxHP) CurrHP = MaxHP;
        }
        else
        {
            MaxHP += gain;
            if (CurrHP > MaxHP) CurrHP = MaxHP;
        }
    }

    public void ManageDamage(int loss, int gain)
    {
        loss = Mathf.Abs(loss);
        gain = Mathf.Abs(gain);

        if (gain > 0)
        {
            Damage += gain;
        }
        else
        {
            Damage -= loss;
        }
    }

    public void ManageArmor(int loss, int gain)
    {
        loss = Mathf.Abs(loss);
        gain = Mathf.Abs(gain);

        if (gain > 0)
        {
            Armor += gain;
        }
        else
        {
            Armor -= loss;
        }
    }

    public Inventory GetInventory()
    {
        return this.Inventory;
    }

    public Inventory GetEquippedItems()
    {
        return this.EquippedItems;
    }

    public void SetEquippedItems(Inventory items)
    {
        this.EquippedItems = items;
    }

    private void CheckIfGameOver()
    {
        if (CurrHP <= 0)
        {
            //TODO: 
            //play particle effect here
            //Call the GameOver function of GameManager.
            //GameManager.instance.GameOver();
        }
    }
}