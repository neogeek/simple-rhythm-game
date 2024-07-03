using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenderSong : MonoBehaviour
{

    [SerializeField]
    private Mesh _mesh;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Material[] _materials;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private float _scale = 2;

    [SerializeField]
    private float _distance = 50;

    private Song _song;

    private readonly Vector3 _noteScale = new(0.5f, 0.25f, 0.5f);

    private readonly Vector3 _beatBarScale = new(5, 0.03f, 0.03f);

    private async void Start()
    {
        _song = Song.FromJSON(await File.ReadAllTextAsync("Assets/Songs/Demo 1/notes.json"));

        _audioSource.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Songs/Demo 1/song.ogg");

        _song.BPM.TryAdd(Song.ConvertSecondToTicks(_audioSource.clip.length, _song.Resolution, _song.sortedBPM),
            _song.BPM.Last().Value);

        _audioSource.Play();
    }

    private void Update()
    {
        if (_song == null)
        {
            return;
        }

        var tickOffset = Song.ConvertSecondToTicks(_audioSource.time, _song.Resolution, _song.sortedBPM);

        for (var x = 0; x < 5; x += 1)
        {
            var noteMatrix = new List<Matrix4x4>();

            foreach (var note in _song.Difficulties[Difficulty.Expert].Where(note => note.HandPosition == x))
            {
                var position = Song.ConvertTickToPosition(note.Position - tickOffset, _song.Resolution) * _scale;

                if (position > _distance)
                {
                    break;
                }

                if (position < 0)
                {
                    continue;
                }

                noteMatrix.Add(Matrix4x4.TRS(new Vector3(note.HandPosition, 0, position),
                    Quaternion.identity, _noteScale));
            }

            Graphics.DrawMeshInstanced(_mesh, 0, _materials[x], noteMatrix);
        }

        var beatBarMatrix = new List<Matrix4x4>();

        foreach (var beatBar in _song.beatBars)
        {
            var position = Song.ConvertTickToPosition(beatBar.Position - tickOffset, _song.Resolution) *
                           _scale;

            if (position > _distance)
            {
                break;
            }

            if (position < 0)
            {
                continue;
            }

            beatBarMatrix.Add(Matrix4x4.TRS(new Vector3(2.5f, 0, position), Quaternion.identity, _beatBarScale));
        }

        Graphics.DrawMeshInstanced(_mesh, 0, _material, beatBarMatrix);
    }

}
