#pragma strict

var materials : Material[];
var materials_counter : int = 0;

var sounds : AudioClip[];
var sounds_counter : int = 0;

function Update()
{
	if(Input.GetKeyDown("f"))
	{
		GetComponent.<Renderer>().material = materials[GetNextMaterialIndex()];
		GetComponent.<AudioSource>().clip = sounds[GetNextSoundIndex()];
		GetComponent.<AudioSource>().Play();
	}
}

function GetNextMaterialIndex() : int
{
	materials_counter++;
	if(materials_counter == materials.Length)
	{
		materials_counter = 0;
	}
	return materials_counter;
}

function GetNextSoundIndex() : int
{
	sounds_counter++;
	if(sounds_counter == sounds.Length)
	{
		sounds_counter = 0;
	}
	return sounds_counter;
}