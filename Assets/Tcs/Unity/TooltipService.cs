using TMPro;
using UnityEngine;

namespace Assets.Tcs.Unity
{
    public class TooltipService : MonoBehaviour
    {
        private static TooltipService _instance;

        public Transform TooltipPanelTransform;
        public TMP_Text TooltipText;

        private bool _isShown = false;
        private Transform _tooltipTextTransform;
        private Vector3 _offset;

        public static TooltipService GetInstance()
        {
            return _instance;
        }

        void Awake()
        {
            TooltipPanelTransform.gameObject.SetActive(false);
            _offset = new Vector3(0, -50, 0);

            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TooltipPanelTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_isShown)
            {
                TooltipPanelTransform.position = Input.mousePosition + _offset;
            }
        }

        public void Show(string text, bool flag = true)
        {
            TooltipText.text = text;
            _isShown = flag;
            TooltipPanelTransform.gameObject.SetActive(flag);
        }
    }
}
