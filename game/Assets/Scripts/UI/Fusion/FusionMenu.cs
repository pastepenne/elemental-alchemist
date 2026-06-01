using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Network;
using ElementalAlchemist.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    /// <summary>
    /// Manages the fusion menu panel opened when interacting with a FusionStation.
    /// </summary>
    public class FusionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _windowPanel;
        [SerializeField] private RecipeCatalog _recipeCatalog;
        [SerializeField] private ElementRegistry _elementRegistry;
        [SerializeField] private ServerConfig _serverConfig;
        [SerializeField] private TMP_Text _statusText;

        [SerializeField] private TierColorPalette _tierColors;
        [SerializeField] private ElementData[] _coreElements;

        [SerializeField] private GameObject _coreSlotPrefab;
        [SerializeField] private GameObject _pouchEntryPrefab;
        [SerializeField] private Transform _coreSlotsContainer;
        [SerializeField] private Transform _pouchEntriesContainer;
        [SerializeField] private FusionIngredientSlot _ingredientSlotA;
        [SerializeField] private FusionIngredientSlot _ingredientSlotB;
        [SerializeField] private Button _fuseButton;
        
        private readonly List<GameObject> _coreSlots = new();
        private readonly List<GameObject> _pouchEntries = new();
        
        private InputAction _cancelAction;
        public static event System.Action Closed;

        private bool _isOpen;
        private bool _isFusing;

        private void Awake()
        {
            _cancelAction = InputSystem.actions.FindAction(InputActions.UI.Cancel);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            FusionStation.Interacted += Open;
            _fuseButton.onClick.AddListener(OnFusePressed);
        }

        private void OnDisable()
        {
            FusionStation.Interacted -= Open;
            _fuseButton.onClick.RemoveListener(OnFusePressed);

            if (_isOpen)
            {
                PlayerManager.Instance.Inventory.ElementAdded -= OnInventoryChanged;
                PlayerManager.Instance.Inventory.ElementRemoved -= OnInventoryChanged;
            }
        }

        private void Open()
        {
            _isOpen = true;
            _backdropPanel.SetActive(true);
            _windowPanel.SetActive(true);
            ActionMapController.SetActionMap(ActionMaps.UI);
            _cancelAction.performed += OnCancel;

            _ingredientSlotA.Clear();
            _ingredientSlotB.Clear();
            _fuseButton.interactable = true;
            SetStatus(string.Empty);

            RefreshCoreElements();
            RefreshPouchElements();

            PlayerManager.Instance.Inventory.ElementAdded += OnInventoryChanged;
            PlayerManager.Instance.Inventory.ElementRemoved += OnInventoryChanged;
        }
        
        private void Close()
        {
            _isOpen = false;
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
            ActionMapController.SetActionMap(ActionMaps.Player);
            _cancelAction.performed -= OnCancel;

            PlayerManager.Instance.Inventory.ElementAdded -= OnInventoryChanged;
            PlayerManager.Instance.Inventory.ElementRemoved -= OnInventoryChanged;

            Closed?.Invoke();
        }
        
        private void RefreshCoreElements()
        {
            foreach (var slot in _coreSlots)
            {
                var component = slot.GetComponent<FusionCoreSlot>();
                if (component)
                {
                    component.ElementSelected -= OnElementSelected;
                }
                
                Destroy(slot);
            }

            _coreSlots.Clear();

            foreach (var element in _coreElements)
            {
                var slotObject = Instantiate(_coreSlotPrefab, _coreSlotsContainer);
                var slot = slotObject.GetComponent<FusionCoreSlot>();
                slot.Setup(element);
                slot.ElementSelected += OnElementSelected;
                _coreSlots.Add(slotObject);
            }
        }
        
        private void RefreshPouchElements()
        {
            foreach (var entry in _pouchEntries)
            {
                var component = entry.GetComponent<FusionPouchEntry>();
                if (component)
                {
                    component.ElementSelected -= OnElementSelected;
                }
                
                Destroy(entry);
            }

            _pouchEntries.Clear();

            var elements = PlayerManager.Instance.Inventory
                .GetStacks()
                .Select(s => s.Element)
                .OrderBy(e => e.Tier)
                .ThenBy(e => e.DisplayName)
                .ToList();
            
            foreach (var element in elements)
            {
                var entryObject = Instantiate(_pouchEntryPrefab, _pouchEntriesContainer);
                var entry = entryObject.GetComponent<FusionPouchEntry>();
                entry.Setup(element);
                entry.ElementSelected += OnElementSelected;
                _pouchEntries.Add(entryObject);
            }
        }
        
        private void OnFusePressed()
        {
            if (_isFusing || !_ingredientSlotA.Current || !_ingredientSlotB.Current)
            {
                return;
            }

            var inputA = _ingredientSlotA.Current;
            var inputB = _ingredientSlotB.Current;

            _isFusing = true;
            _fuseButton.interactable = false;
            SetStatus("Fusing...");
            StartCoroutine(FusionService.Fuse(_recipeCatalog, _elementRegistry, _serverConfig.BaseUrl, inputA, inputB, OnFusionComplete));
        }

        private void OnFusionComplete(FusionResult result)
        {
            _isFusing = false;
            _fuseButton.interactable = true;
            ShowResult(result);
            _ingredientSlotA.Clear();
            _ingredientSlotB.Clear();
        }

        private void ShowResult(FusionResult result)
        {
            if (result.Success)
            {
                SetStatus(result.Output.DisplayName);
            }
            else
            {
                SetStatus("Nothing happens...");
            }
        }

        private void SetStatus(string message)
        {
            if (_statusText)
            {
                _statusText.text = message;
            }
        }
        
        private void OnInventoryChanged(ElementData _)
        {
            RefreshPouchElements();
        }
        
        private void OnElementSelected(ElementData element)
        {
            if (_ingredientSlotA.Current == element || _ingredientSlotB.Current == element)
            {
                return;
            }
            
            if (!_ingredientSlotA.Current)
            {
                _ingredientSlotA.Setup(element);
            }
            else if (!_ingredientSlotB.Current)
            {
                _ingredientSlotB.Setup(element);
            }
        }
        
        private void OnCancel(InputAction.CallbackContext context)
        {
            Close();
        }
    }
}
