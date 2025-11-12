using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using viperOSK;

namespace viperOSK_Examples
{

    public class Example11 : MonoBehaviour
    {

        public Vector2 resize = new Vector2(600f, 400f);

        public Vector2 margins = new Vector2(10f, 10f);

        public OSK_UI_Keyboard keyboard;
        public GameObject inputField;

        public RectTransform keyboardFrame;

        public RectTransform resizeTarget;
        public RectTransform viewportTarget;

        [TextArea(5,10)]
        public string portraitLayout = "1 2 3 4 5 6 7 8 9 0 + - \n Q W E R T Y U I O P Backspace \n Skip.2 CapsLock A S D F G H J K L ! \n LeftShift Z X C V B N M ? RightShift \n Skip.2 LeftControl Space . _ Return";

        [TextArea(5, 10)]
        public string landscapeLayout = "Q W E R T Y U I O P Skip.5 7 8 9 / \n A S D F G H J K L @ Skip.5 4 5 6 * \n Z X C V B N M _ Backspace Skip.5 1 2 3 - \n LeftShift Space Return Skip.5 0 . +";

        bool isLandscape;

        bool isResizable = false;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void ResizeBtnCallback()
        {
            isResizable = !isResizable;

            if (isResizable)
            {
                ResizeBasedOnTarget();
            }
            else
            {
                isLandscape = Screen.width > Screen.height;
                ResizeBasedOnRect(viewportTarget);
            }
        }

        public void ResizeBasedOnRectViewport(UIResizeReact.ViewportInfo info)
        {
            Debug.Log("viewport resize");
            isResizable = false;
            isLandscape = Screen.width > Screen.height;
            ResizeBasedOnRect(info.target);
        }

        public void ResizeBasedOnTarget()
        {
            isResizable = true;
            isLandscape = resizeTarget.rect.width > resizeTarget.rect.height;

            Debug.Log("target w=" + resizeTarget.rect.width + " / h=" + resizeTarget.rect.height);
            ResizeBasedOnRect(resizeTarget);

        }

        public void ResizeBasedOnRect(RectTransform targetobj)
        {

            StartCoroutine(Resize(targetobj));

        }

        public IEnumerator Resize(RectTransform targetobj)
        {


            yield return new WaitForEndOfFrame();
            yield return new WaitForSecondsRealtime(.05f);

            resize.x = targetobj.rect.width - margins.x * 2f;
            resize.y = targetobj.rect.height - margins.y * 2f;

            Debug.Log("[viperOSK: Ex11] new size = " + resize.ToString());

            var corners = new Vector3[4];
            targetobj.GetWorldCorners(corners);

            keyboard.topLeft.GetComponent<RectTransform>().position = new Vector3(corners[1].x + margins.x, corners[1].y - margins.y, keyboard.topLeft.transform.position.z);

            // reposition inputfield so it's always on top of keyboard
            inputField.transform.position = new Vector3( .5f*(corners[1].x + corners[2].x),
                corners[1].y + inputField.GetComponent<RectTransform>().rect.height,
                inputField.transform.position.z);

            
            keyboard.LoadLayout(isLandscape ? landscapeLayout : portraitLayout);

            if(isLandscape)
            {
                OSK_Settings.instance.physicalKeyboardLayout = "QWERTYUIOP[]\\nASDFGHJKL;'\nZXCVBNM,./";
            } else
            {
                OSK_Settings.instance.physicalKeyboardLayout = "1234567890-=\nQWERTYUIOP[]\\nASDFGHJKL;'\nZXCVBNM,./";
            }

            keyboard.ResizeKeyToFit(resize);
            keyboard.Reset();

            //yield return new WaitForSecondsRealtime(.05f);
            // do animation?


            keyboard.Generate();
            //keyboard.RemapPhysicalKeyboard();

            OSK_Background bg = keyboard.transform.GetComponentInChildren<OSK_Background>();
            if (bg != null)
            {
                bg.ResizeToFit();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
