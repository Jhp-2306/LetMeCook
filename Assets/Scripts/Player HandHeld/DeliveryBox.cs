using Constants;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace HandHeld
{
    public class DeliveryBox : MonoBehaviour, IHandHeld, IInteractable
    {
        private bool isInteractable;

        public GameObject Hud;
        public Image _Slider;
        public TMPro.TextMeshProUGUI Timertxt;

        BasicStorageSystem<IStorageItem> storageSystem;
        private void Start()
        {
            //if (storageSystem == null)
            //{
            //    storageSystem = new BasicStorageSystem<IStorageItem>(4);
            //}
            //AddBoxItem(new RefrigeratorItems(IngredientType.Tomato, 20));
            //AddBoxItem(new RefrigeratorItems(IngredientType.Chicken, 2));
            //AddBoxItem(new RefrigeratorItems(IngredientType.Carrot, 7));
            //AddBoxItem(new RefrigeratorItems(IngredientType.BurgerBun, 8));
        }

        public void init(int cap, float timerInSec)
        {
            if (storageSystem == null)
            {
                storageSystem = new BasicStorageSystem<IStorageItem>(cap);
            }
            if (timerInSec > 0)
            {
                Hud.SetActive(true);
                isInteractable = false;
                StartCoroutine(timer(timerInSec));
            }
            else
            {
                isInteractable = true;
                Hud.SetActive(false);
            }
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

        public void OnClick()
        {
            //open the box
            Debug.Log("Open Deliver Box");
            HUDManagerDNDL.Instance.OpenInventory(storageSystem);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (isInteractable)
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
            return isInteractable;
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

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public IEnumerator timer(float timerInSec)
        {
            float timer = timerInSec;
            while (timer >= 0)
            {
                yield return null;
                timer -= Time.deltaTime;
                _Slider.fillAmount = ((timerInSec - timer) / timerInSec);
                Timertxt.text = TimeManagementDNDL.Instance.GetTimerinFormate(timer);
                //Debug.Log($"{TimeManagementDNDL.Instance.GetTimerinFormate(timer)}");
            }
            isInteractable = true;
            Hud.SetActive(false);
        }
    }
}

