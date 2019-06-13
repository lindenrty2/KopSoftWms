using YL.Core.Entity;
using YL.Utils.Table;

namespace IServices
{
    public interface IWms_mastaskServices: IBaseServices<Wms_mestask>
    {
        string PageList(Bootstrap.BootstrapParams bootstrap);
    }
}
