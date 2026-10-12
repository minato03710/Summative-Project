using UnityEngine;
using UnityEngine.VFX;

[DisallowMultipleComponent]
[RequireComponent(typeof(VisualEffect))]
public class SimpleVFX : MonoBehaviour
{
    private VisualEffect effect;
    private bool playing;
    private float remaining = -1f;

    private static SimpleVFX GetPlayer(VisualEffect vfx)
    {
        if (vfx == null)
            return null;

        SimpleVFX player = vfx.GetComponent<SimpleVFX>();

        if (player == null)
            player = vfx.gameObject.AddComponent<SimpleVFX>();

        player.effect = vfx;
        return player;
    }

    public static void Prepare(VisualEffect vfx)
    {
        SimpleVFX player = GetPlayer(vfx);

        if (player != null)
            player.Clear();
    }

    public static void Pulse(
        VisualEffect vfx,
        float duration = 0.2f)
    {
        SimpleVFX player = GetPlayer(vfx);

        if (player == null)
            return;

        player.Begin();
        player.remaining = Mathf.Max(0.01f, duration);
    }

    public static void Loop(VisualEffect vfx, bool active)
    {
        SimpleVFX player = GetPlayer(vfx);

        if (player == null)
            return;

        if (active)
        {
            if (!player.playing)
                player.Begin();

            player.remaining = -1f;
        }
        else if (player.playing)
        {
            player.Stop();
        }
    }

    // 独立生成特效，角色死亡或传送后仍可以播完。
    public static void Spawn(
        VisualEffect template,
        float duration = 0.2f,
        float fadeOutTime = 2f)
    {
        if (template == null)
            return;

        VisualEffect copy = Instantiate(
            template,
            template.transform.position,
            template.transform.rotation
        );

        copy.transform.localScale = template.transform.lossyScale;
        copy.gameObject.SetActive(true);
        copy.enabled = true;

        Pulse(copy, duration);

        Destroy(
            copy.gameObject,
            Mathf.Max(0.01f, duration) +
            Mathf.Max(0f, fadeOutTime)
        );
    }

    private void Awake()
    {
        effect = GetComponent<VisualEffect>();
        Clear();
    }

    private void Begin()
    {
        effect.initialEventName = "Manual";
        effect.enabled = true;
        effect.pause = Time.timeScale <= 0f;

        effect.Reinit();
        effect.Play();

        playing = true;
    }

    private void Update()
    {
        if (effect == null)
            return;

        effect.pause = Time.timeScale <= 0f;

        if (effect.pause || !playing || remaining < 0f)
            return;

        remaining -= Time.deltaTime;

        if (remaining <= 0f)
            Stop();
    }

    private void Stop()
    {
        if (effect != null)
            effect.Stop();

        playing = false;
        remaining = -1f;
    }

    private void Clear()
    {
        Stop();

        if (effect == null)
            return;

        effect.initialEventName = "Manual";
        effect.Reinit();
    }

    private void OnDisable()
    {
        Clear();
    }
}