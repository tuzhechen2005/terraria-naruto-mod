using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using ShinobiPrototype.Common.Players;

namespace ShinobiPrototype.Common.Systems;

// The first test of the Chūnin Exams (specs/M2_中忍考试篇.spec.md 3.1): nine questions from WrittenExamBank, answers
// shuffled and never marked, then Ibiki's tenth question, where accepting is the pass.
public sealed class WrittenExamSystem : ModSystem
{
    private static UserInterface ui;
    private static WrittenExamState state;

    public static bool IsOpen => ui?.CurrentState != null;

    public override void Load()
    {
        if (Main.dedServ)
            return;
        ui = new UserInterface();
        state = new WrittenExamState();
    }

    public override void Unload()
    {
        ui = null;
        state = null;
    }

    public static void Open()
    {
        if (ui == null || IsOpen)
            return;
        HandbookSystem.Close();
        state.Begin();
        ui.SetState(state);
        SoundEngine.PlaySound(SoundID.MenuOpen);
    }

    public static void Close()
    {
        if (!IsOpen)
            return;
        ui.SetState(null);
        SoundEngine.PlaySound(SoundID.MenuClose);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (!IsOpen)
            return;
        if (Main.gameMenu || Main.LocalPlayer.dead)
            ui.SetState(null);
        else
            ui.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        layers.Insert(index >= 0 ? index : layers.Count, new LegacyGameInterfaceLayer(
            "ShinobiPrototype: Written Exam",
            () =>
            {
                if (IsOpen)
                    ui.Draw(Main.spriteBatch, new GameTime());
                return true;
            },
            InterfaceScaleType.UI));
    }
}

internal sealed class WrittenExamState : UIState
{
    private static readonly Color Idle = new Color(63, 82, 151) * 0.9f;
    private static readonly Color Hover = new(90, 110, 190);

    private UIPanel panel;
    private UIText header;
    private UIElement content;
    private int[] questions;
    private int index;
    private int correct;

    public override void OnInitialize()
    {
        panel = new UIPanel();
        panel.Width.Set(640f, 0f);
        panel.Height.Set(430f, 0f);
        panel.HAlign = 0.5f;
        panel.VAlign = 0.5f;
        Append(panel);

        header = new UIText("", 1f);
        header.Top.Set(4f, 0f);
        panel.Append(header);

        UITextPanel<string> close = new("×", 0.9f);
        close.Width.Set(36f, 0f);
        close.HAlign = 1f;
        close.OnLeftClick += (_, _) =>
        {
            if (index < ChuninExamRules.WrittenQuestions)
                Main.NewText("你离开了考场。笔试随时可以重新开始。", 200, 200, 200);
            WrittenExamSystem.Close();
        };
        panel.Append(close);

        content = new UIElement();
        content.Top.Set(44f, 0f);
        content.Width.Set(0f, 1f);
        content.Height.Set(-44f, 1f);
        panel.Append(content);
    }

    public void Begin()
    {
        questions = ChuninExamRules.Draw(WrittenExamBank.Questions.Length, ChuninExamRules.WrittenQuestions, Main.rand.Next);
        index = 0;
        correct = 0;
        ShowQuestion();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (panel.ContainsPoint(Main.MouseScreen))
            Main.LocalPlayer.mouseInterface = true;
    }

    private void ShowQuestion()
    {
        content.RemoveAllChildren();
        if (index >= questions.Length)
        {
            ShowTenth();
            return;
        }
        ExamQuestion question = WrittenExamBank.Questions[questions[index]];
        header.SetText($"中忍考试 · 第一试　　第 {index + 1} / 10 题");
        AddText(question.Text, 0f, 1f);
        int[] order = ChuninExamRules.Draw(question.Options.Length, question.Options.Length, Main.rand.Next);
        for (int i = 0; i < order.Length; i++)
        {
            int option = order[i];
            AddButton($"{(char)('A' + i)}. {question.Options[option]}", 70f + i * 52f, () =>
            {
                if (option == 0)
                    correct++;
                index++;
                SoundEngine.PlaySound(SoundID.MenuTick);
                ShowQuestion();
            });
        }
    }

    private void ShowTenth()
    {
        header.SetText("中忍考试 · 第一试　　第 10 题");
        AddText(WrittenExamBank.TenthIntro, 0f, 0.9f);
        AddButton("接受第十题", 200f, () =>
        {
            Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>().PassWritten(correct);
            SoundEngine.PlaySound(SoundID.Item4);
            ShowEnd(WrittenExamBank.TenthAccepted);
        });
        AddButton("放弃", 256f, () =>
        {
            Main.LocalPlayer.GetModPlayer<ChuninExamPlayer>().GiveUpWritten();
            Main.NewText("你在第十题放弃了。明天天亮后可以再来重考。", 250, 200, 120);
            ShowEnd(WrittenExamBank.TenthGaveUp);
        });
    }

    private void ShowEnd(string text)
    {
        content.RemoveAllChildren();
        header.SetText("中忍考试 · 第一试");
        AddText(text, 0f, 0.9f);
        AddButton("离开考场", 250f, WrittenExamSystem.Close);
    }

    private void AddText(string text, float top, float scale)
    {
        UIText body = new(text, scale)
        {
            IsWrapped = true,
            TextOriginX = 0f,
            TextOriginY = 0f,
        };
        body.Top.Set(top, 0f);
        body.Width.Set(0f, 1f);
        body.Height.Set(180f, 0f);
        content.Append(body);
    }

    private void AddButton(string text, float top, System.Action onClick)
    {
        UITextPanel<string> button = new(text, 0.9f)
        {
            BackgroundColor = Idle,
            TextHAlign = 0f,
        };
        button.Top.Set(top, 0f);
        button.Width.Set(0f, 1f);
        button.Height.Set(44f, 0f);
        button.OnMouseOver += (_, _) => button.BackgroundColor = Hover;
        button.OnMouseOut += (_, _) => button.BackgroundColor = Idle;
        button.OnLeftClick += (_, _) => onClick();
        content.Append(button);
    }
}
