// dllmain.h : Declaration of module class.

class CFMInterfacesModule : public CAtlDllModuleT< CFMInterfacesModule >
{
public :
	DECLARE_LIBID(LIBID_FMInterfacesLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_FMINTERFACES, "{876157A4-4DC8-47BF-BCB3-546E7D874400}")
};

extern class CFMInterfacesModule _AtlModule;
