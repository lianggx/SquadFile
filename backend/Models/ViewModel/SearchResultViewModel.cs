namespace SquadFile.Models.ViewModel
{
    /// <summary>
    /// 搜索结果DTO
    /// </summary>
    public class SearchResultViewModel
    {
        /// <summary>
        /// 文件夹列表
        /// </summary>
        public List<FileFolderViewModel> Folders { get; set; } = new List<FileFolderViewModel>();

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileRecordViewModel> Files { get; set; } = new List<FileRecordViewModel>();
    }
}