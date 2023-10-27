using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
	[SerializeField] private GlobalValuesScriptableObject globalValuesScriptableObject;
	public override void InstallBindings()
	{
		Container.Bind<GlobalValuesScriptableObject>().FromScriptableObject(globalValuesScriptableObject).AsSingle().NonLazy();
	}
}
