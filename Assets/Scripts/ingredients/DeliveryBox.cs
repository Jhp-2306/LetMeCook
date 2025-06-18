using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandHeld
{
    public class DeliveryBox : MonoBehaviour, IHandHeld, IInteractable
    {

        BasicStorageSystem<IStorageItem> storageSystem;
        private void Start()
        {
            if (storageSystem == null)
            {
                storageSystem = new BasicStorageSystem<IStorageItem>(4);
            }
            AddBoxItem(new RefrigeratorItems(IngredientType.Tomato, 20));
            AddBoxItem(new RefrigeratorItems(IngredientType.Apple, 2));
            AddBoxItem(new RefrigeratorItems(IngredientType.Mango, 7));
        }

        public void AddBoxItem(RefrigeratorItems boxItem)
        {
            storageSystem.AddItems(boxItem);
        }

        public void AddEvent()
        {
            InputManager.Instance.InteractionButtonAddEvent("Open", () => { OnClick(); });
        }

        public SO_EquipmentData GetEquipmentData()
        {
            throw new System.NotImplementedException();
        }

        public EquipmentType GetEquipmentType()
        {
            throw new System.NotImplementedException();
        }

        public typeofhandheld IGetType() => typeofhandheld.box;

        public void OnClick(bool ishandfull = false)
        {
            //open the box
            Debug.Log("Open Deliver Box");
            HUDManagerDNDL.Instance.OpenInventory(storageSystem);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other != null)
            {
                if (other.gameObject.tag == "Player")
                {
                    if (other.gameObject.GetComponent<Player>().isPlayerHandEmpty)
                    {
                        other.gameObject.GetComponent<Player>().PickSomeThing(this, this.gameObject);
                        AddEvent();

                    }
                }
            }
        }

        public bool IsInteractable()
        {
            throw new System.NotImplementedException();
        }


        public IStorageItem CustomEventReturner()
        {
            if (storageSystem.GetAllItems().Count > 0)
            { 
            var temp = storageSystem.GetItemAtIndex(0);
            storageSystem.RemoveItemAt(0);
            return temp;
            }
            return null;
        }

        public void DestroyMe()
        {
            Destroy(this.gameObject);
        }

        object IHandHeld.CustomEventReturner()
        {
            return CustomEventReturner();
        }
    }
}

