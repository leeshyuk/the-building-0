/*
 * 
 *      프리팹 Signboard 1, Signboard 2의 스크립트
 *      
 *      *기능
 *      1.  초기 글자들 설정 가능
 *      2.  이상 현상으로 글자와 글자색이 바뀌는 것 구현
 *      
 *      
 *      *주의 사항
 *      1.  글자가 너무 길면 오브젝트를 벗어남
 *      
 */
using UnityEngine;
using TMPro;

public class SignboardController : MonoBehaviour
{   

    // 간판이 양면이므로 같은 TextMeshPro 오브젝트가 2개씩 있음
    public TextMeshPro[] number = new TextMeshPro[2];
    public TextMeshPro[] english = new TextMeshPro[2];
    public TextMeshPro[] korean = new TextMeshPro[2];

    //초기 글자들
    public string initialNumber = "401";
    public string initialEnglish = "Lecture Room";
    public string initialKorean = "강의실";

    // 이상 현상 on/off
    public bool strangeState = false;

    // 이상 현상 시 글자 색
    public Color strangeTextColor = Color.red;

    // 이상 현상 시 글자들
    public string strangeNumber = "444";
    public string strangeEnglish = "DIE";
    public string strangeKorean = "죽어";

    // 위의 변수들은 기본값일 뿐 유니티 에디터에서 수정 가능

    private void Update()
    {
        // 이상 상태일 때
        if (strangeState)
        {
            // 글자 바꾸기
            ChangeText(number, strangeNumber);
            ChangeText(english, strangeEnglish);
            ChangeText(korean, strangeKorean);

            // 글자색 바꾸기
            ChangeColor(number, strangeTextColor);
            ChangeColor(english, strangeTextColor);
            ChangeColor(korean, strangeTextColor);
        }
        else // 이상 상태 아닐 때
        {
            // 글자 원래대로
            ChangeText(number, initialNumber);
            ChangeText(english, initialEnglish);
            ChangeText(korean, initialKorean);
            // 글자색 원래대로
            ChangeColor(number, new Color(0f, 45 / 255f, 87 / 255f)); // 강의실 번호는 원래색이 하얀색이 아님
            ChangeColor(english, Color.white);
            ChangeColor(korean, Color.white);
        }
    }

    // 글자를 바꾸는 메소드
    // textMeshPros: 글자를 바꿀 오브젝트들, text: 목표 글자
    // textMeshPros의 원소의 text를 text로 바꿈
    private void ChangeText(TextMeshPro[] textMeshPros, string text)
    {
        foreach (var textMeshPro in textMeshPros)
        {
            textMeshPro.text = text;
        }
    }

    // 글자색을 바꾸는 메소드
    // textMeshPros: 글자를 바꿀 오브젝트들, textColor: 목표 글자색
    // textMeshPros의 원소의 color와 colorGradient를 textColor로 바꿈
    // colorGradient를 바꾸지 않으면 글자색이 목표하는 색보다 연하게 나타나는 현상이 있음.
    private void ChangeColor(TextMeshPro[] textMeshPros, Color textColor)
    {
        foreach (var textMeshPro in textMeshPros)
        {
            textMeshPro.color = textColor;
            textMeshPro.colorGradient = new VertexGradient(textColor);
        }
    }
}
