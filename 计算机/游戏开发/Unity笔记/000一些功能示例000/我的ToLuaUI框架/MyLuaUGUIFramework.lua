
function Main()
    UpdateBeat:Add(Update, self);

    button1 = UnityEngine.GameObject("Button1");
    InitButton(button1);

end

function InitButton(go)
    go.transform:SetParent(canvas1.transform);
    local btnButton1 = go:AddComponent(typeof(UnityEngine.UI.Button));
    local rectButton1 = go:AddComponent(typeof(UnityEngine.RectTransform));
    local imgButton1 = go:AddComponent(typeof(UnityEngine.UI.Image));
    rectButton1.anchoredPosition3D = Vector3(0, 0, 0);
    rectButton1.localScale = Vector3(1, 1, 1);

    local delegate = UnityEngine.Events.UnityAction(ButtonClick);
    btnButton1.onClick:AddListener(delegate);
end

function Destroy(go)
    UnityEngine.Object.Destroy(go);
end

function ButtonClick()
    print("click!!!!!!!!!!");
    Destroy(button1);
end



Main();




