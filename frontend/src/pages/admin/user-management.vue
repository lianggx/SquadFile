<template>
  <div class="user-management-container">
    <!-- 添加 Toast 组件 -->
    <Toast 
      :message="toastMessage" 
      :type="toastType" 
      :duration="3000" 
    />
    
    <h1 class="title">{{ t('admin.userManagement') }}</h1>
    
    <div class="toolbar">
      <button @click="showCreateUserModal" class="create-button">
        {{ t('admin.createUser') }}
      </button>
    </div>
    
    <!-- 桌面端表格视图 -->
    <div class="users-table desktop-table">
      <table>
        <thead>
          <tr>
            <th>{{ t('admin.userId') }}</th>
            <th>{{ t('admin.username') }}</th>
            <th>{{ t('admin.displayName') }}</th>
            <th>{{ t('admin.email') }}</th>
            <th>{{ t('admin.department') }}</th>
            <th>{{ t('admin.role') }}</th>
            <th>{{ t('admin.status') }}</th>
            <th>{{ t('admin.actions') }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.username }}</td>
            <td>{{ user.displayName || '-' }}</td>
            <td>{{ user.email || '-' }}</td>
            <td>{{ user.department || '-' }}</td>
            <td>{{ getUserRoleText(user.role) }}</td>
            <td>{{ getUserStatusText(user.status) }}</td>
            <td>
              <button @click="showEditUserModal(user)" class="action-button edit-button">
                {{ t('admin.edit') }}
              </button>
              <button @click="showFolderPermissionsModal(user)" class="action-button permission-button">
                {{ t('admin.folderPermissions') }}
              </button>
              <button @click="showResetPasswordModal(user)" class="action-button reset-password-button">
                {{ t('admin.resetPassword') }}
              </button>
              <button @click="deleteUser(user)" class="action-button delete-button">
                {{ t('admin.delete') }}
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <!-- 移动端卡片视图 -->
    <div class="mobile-users-list">
      <div v-for="user in users" :key="user.id" class="user-card">
        <div class="user-card-header">
          <h3>{{ user.username }}</h3>
          <span class="user-id">#{{ user.id }}</span>
        </div>
        <div class="user-card-body">
          <div class="user-info">
            <span class="info-label">{{ t('admin.displayName') }}:</span>
            <span class="info-value">{{ user.displayName || '-' }}</span>
          </div>
          <div class="user-info">
            <span class="info-label">{{ t('admin.email') }}:</span>
            <span class="info-value">{{ user.email || '-' }}</span>
          </div>
          <div class="user-info">
            <span class="info-label">{{ t('admin.department') }}:</span>
            <span class="info-value">{{ user.department || '-' }}</span>
          </div>
          <div class="user-info">
            <span class="info-label">{{ t('admin.role') }}:</span>
            <span class="info-value">{{ getUserRoleText(user.role) }}</span>
          </div>
          <div class="user-info">
            <span class="info-label">{{ t('admin.status') }}:</span>
            <span class="info-value">{{ getUserStatusText(user.status) }}</span>
          </div>
        </div>
        <div class="user-card-actions">
          <button @click="showEditUserModal(user)" class="action-button edit-button">
            {{ t('admin.edit') }}
          </button>
          <button @click="showFolderPermissionsModal(user)" class="action-button permission-button">
            {{ t('admin.folderPermissions') }}
          </button>
          <button @click="showResetPasswordModal(user)" class="action-button reset-password-button">
            {{ t('admin.resetPassword') }}
          </button>
          <button @click="deleteUser(user)" class="action-button delete-button">
            {{ t('admin.delete') }}
          </button>
        </div>
      </div>
    </div>
    
    <!-- 创建用户模态框 -->
    <div v-if="showCreateModal" class="modal">
      <div class="modal-content">
        <h2>{{ t('admin.createUser') }}</h2>
        <form @submit.prevent="createUser">
          <div class="form-group">
            <label>{{ t('admin.username') }}:</label>
            <input v-model="newUser.username" type="text" required />
          </div>
          <div class="form-group">
            <label>{{ t('admin.email') }}:</label>
            <input v-model="newUser.email" type="email" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.displayName') }}:</label>
            <input v-model="newUser.displayName" type="text" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.department') }}:</label>
            <input v-model="newUser.department" type="text" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.role') }}:</label>
            <select v-model="newUser.role">
              <option value="Normal">{{ t('admin.normalUser') }}</option>
              <option value="Admin">{{ t('admin.adminUser') }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>{{ t('admin.password') }}:</label>
            <input v-model="newUser.password" type="password" required />
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.save') }}</button>
            <button type="button" @click="closeCreateModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 编辑用户模态框 -->
    <div v-if="showEditModal" class="modal">
      <div class="modal-content">
        <h2>{{ t('admin.editUser') }}</h2>
        <form @submit.prevent="updateUser">
          <div class="form-group">
            <label>{{ t('admin.username') }}:</label>
            <input v-model="editingUser.username" type="text" required />
          </div>
          <div class="form-group">
            <label>{{ t('admin.email') }}:</label>
            <input v-model="editingUser.email" type="email" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.displayName') }}:</label>
            <input v-model="editingUser.displayName" type="text" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.department') }}:</label>
            <input v-model="editingUser.department" type="text" />
          </div>
          <div class="form-group">
            <label>{{ t('admin.role') }}:</label>
            <select v-model="editingUser.role">
              <option value="Normal">{{ t('admin.normalUser') }}</option>
              <option value="Admin">{{ t('admin.adminUser') }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>{{ t('admin.status') }}:</label>
            <select v-model="editingUser.status">
              <option value="Active">{{ t('admin.active') }}</option>
              <option value="Inactive">{{ t('admin.inactive') }}</option>
              <option value="Locked">{{ t('admin.locked') }}</option>
            </select>
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.save') }}</button>
            <button type="button" @click="closeEditModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 重置密码模态框 -->
    <div v-if="showResetPasswordModalFlag" class="modal">
      <div class="modal-content">
        <h2>{{ t('admin.resetPassword') }}</h2>
        <form @submit.prevent="resetPassword">
          <div class="form-group">
            <label>{{ t('admin.username') }}: {{ resetPasswordUser.username }}</label>
          </div>
          <div class="form-group">
            <label>{{ t('admin.enterNewPassword') }}:</label>
            <input v-model="resetPasswordForm.newPassword" type="password" required />
          </div>
          <div class="form-actions">
            <button type="submit" class="save-button">{{ t('admin.save') }}</button>
            <button type="button" @click="closeResetPasswordModal" class="cancel-button">{{ t('admin.cancel') }}</button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- 文件夹权限管理模态框 -->
    <div v-if="showFolderPermissionsModalFlag" class="modal">
      <div class="modal-content permission-modal">
        <h2>{{ t('admin.folderPermissionsForUser', { username: selectedUser.username }) }}</h2>
        <div class="permission-content">
          <!-- 文件夹树状列表 -->
          <div class="folder-tree">
            <h3>{{ t('admin.allFolders') }}</h3>
            <!-- <div class="folder-tree-actions">
              <button @click="saveFolderPermissionsBatch" class="save-button">
                {{ t('admin.save') }}
              </button>
              <button @click="revokeAllPermissions" class="delete-button">
                {{ t('admin.revokeAll') }}
              </button>
              <button @click="closeFolderPermissionsModal" class="cancel-button">
                {{ t('admin.close') }}
              </button>
            </div> -->
            <div class="folder-search">
              <input 
                v-model="folderSearchTerm" 
                :placeholder="t('admin.searchFolders')" 
                class="search-input"
              />
            </div>
            <div class="folder-tree-container">
              <!-- 使用递归方式渲染文件夹树 -->
              <div v-for="folder in folderTree" :key="folder.id">
                <FolderTree 
                  :folder="folder"
                  :level="0"
                  @toggle-folder="toggleFolder"
                  @toggle-permission="toggleFolderPermission"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="modal-actions">
          <button @click="saveFolderPermissionsBatch" class="save-button">
            {{ t('admin.save') }}
          </button>
          <button @click="revokeAllPermissions" class="delete-button">
            {{ t('admin.revokeAll') }}
          </button>
          <button @click="closeFolderPermissionsModal" class="cancel-button">
            {{ t('admin.close') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { usePageAuth } from '../../composables/useAuth'

// 导入 Toast 组件
import Toast from '../../components/Toast.vue'
import FolderTree from '../../components/FolderTree.vue'

const { t } = useI18n()
const { checkAuthOnMounted } = usePageAuth()

// 导入批量权限操作API
import { 
  getAllUsersApi, 
  createUserApi, 
  updateUserApi, 
  deleteUserApi, 
  resetUserPasswordApi 
} from '../../services/user'
import { 
  getUserFoldersApi,
  getFolderPermissionsApi,
  grantFolderPermissionApi,
  revokeFolderPermissionApi,
  grantFolderPermissionsBatchApi,
  revokeFolderPermissionsBatchApi
} from '../../services/file'

// Toast 相关数据
const toastMessage = ref('')
const toastType = ref('info')

// 页面加载时检查认证状态
checkAuthOnMounted(false)

// 用户数据
const users = ref([])

// 文件夹数据
const folders = ref([])
const folderPermissions = ref([])

// 文件夹树状结构数据
const folderTree = ref([])

// 模态框状态
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showResetPasswordModalFlag = ref(false)
const showFolderPermissionsModalFlag = ref(false)
const showPermissionEditModalFlag = ref(false)

// 当前选中的用户和文件夹
const selectedUser = ref({})
const editingFolder = ref(null)

// 搜索词
const folderSearchTerm = ref('')

// 权限选项
const permissionOptions = ref({
  canRead: true,
  canUpload: false,
  canDelete: false
})

// 新用户表单数据
const newUser = ref({
  username: '',
  email: '',
  displayName: '',
  department: '',
  role: 'Normal',
  password: ''
})

// 编辑中的用户数据
const editingUser = ref({
  id: null,
  username: '',
  email: '',
  displayName: '',
  department: '',
  role: 'Normal',
  status: 'Active'
})

// 重置密码相关数据
const resetPasswordUser = ref({})
const resetPasswordForm = ref({
  newPassword: ''
})

// 页面加载时获取用户列表
onMounted(() => {
  fetchUsers()
  fetchFolders()
})

// 获取所有用户
const fetchUsers = async () => {
  try {
    const response = await getAllUsersApi()
    users.value = response.data.data || []
  } catch (error) {
    console.error('获取用户列表失败:', error)
    showToast(t('admin.fetchUsersFailed'), 'error')
  }
}

// 获取所有文件夹
const fetchFolders = async () => {
  try {
    const response = await getUserFoldersApi()
    folders.value = response.data.data || []
    // 构建树状结构
    folderTree.value = buildFolderTree(folders.value)
  } catch (error) {
    console.error('获取文件夹列表失败:', error)
    showToast(t('admin.fetchFoldersFailed'), 'error')
  }
}

// 获取文件夹权限
const fetchFolderPermissions = async (folderId) => {
  try {
    const response = await getFolderPermissionsApi(folderId)
    folderPermissions.value = [...folderPermissions.value, ...(response.data.data || [])]
  } catch (error) {
    console.error('获取文件夹权限失败:', error)
    showToast(t('admin.fetchPermissionsFailed'), 'error')
  }
}

// 显示创建用户模态框
const showCreateUserModal = () => {
  // 重置表单
  newUser.value = {
    username: '',
    email: '',
    displayName: '',
    department: '',
    role: 'Normal',
    password: ''
  }
  showCreateModal.value = true
}

// 关闭创建用户模态框
const closeCreateModal = () => {
  showCreateModal.value = false
}

// 创建用户
const createUser = async () => {
  try {
    await createUserApi(newUser.value)
    closeCreateModal()
    await fetchUsers()
    showToast(t('admin.userCreated'), 'success')
  } catch (error) {
    console.error('创建用户失败:', error)
    showToast(t('admin.createUserFailed'), 'error')
  }
}

// 显示编辑用户模态框
const showEditUserModal = (user) => {
  editingUser.value = {
    id: user.id,
    username: user.username,
    email: user.email || '',
    displayName: user.displayName || '',
    department: user.department || '',
    role: user.role,
    status: user.status || 'Active'
  }
  showEditModal.value = true
}

// 关闭编辑用户模态框
const closeEditModal = () => {
  showEditModal.value = false
}

// 更新用户
const updateUser = async () => {
  try {
    await updateUserApi(editingUser.value.id, editingUser.value)
    closeEditModal()
    await fetchUsers()
    showToast(t('admin.userUpdated'), 'success')
  } catch (error) {
    console.error('更新用户失败:', error)
    showToast(t('admin.updateUserFailed'), 'error')
  }
}

// 删除用户
const deleteUser = async (user) => {
  if (confirm(t('admin.confirmDeleteUser', { username: user.username }))) {
    try {
      await deleteUserApi(user.id)
      await fetchUsers()
      showToast(t('admin.userDeleted'), 'success')
    } catch (error) {
      console.error('删除用户失败:', error)
      showToast(t('admin.deleteUserFailed'), 'error')
    }
  }
}

// 显示重置密码模态框
const showResetPasswordModal = (user) => {
  resetPasswordUser.value = user
  resetPasswordForm.value.newPassword = ''
  showResetPasswordModalFlag.value = true
}

// 关闭重置密码模态框
const closeResetPasswordModal = () => {
  showResetPasswordModalFlag.value = false
}

// 重置用户密码
const resetPassword = async () => {
  try {
    await resetUserPasswordApi(resetPasswordUser.value.id, resetPasswordForm.value.newPassword)
    closeResetPasswordModal()
    showToast(t('admin.passwordReset'), 'success')
  } catch (error) {
    showToast(t('admin.resetPasswordFailed'), 'error')
  }
}

// 显示文件夹权限模态框
const showFolderPermissionsModal = async (user) => {
  selectedUser.value = user
  showFolderPermissionsModalFlag.value = true
  
  // 获取所有文件夹的权限信息
  folderPermissions.value = []
  for (const folder of folders.value) {
    await fetchFolderPermissions(folder.id)
  }
  
  // 构建带权限的文件夹树
  folderTree.value = buildFolderTreeWithPermissions(
    folders.value, 
    folderPermissions.value, 
    user.id
  )
}

// 关闭文件夹权限模态框
const closeFolderPermissionsModal = () => {
  showFolderPermissionsModalFlag.value = false
  selectedUser.value = {}
}

// 显示权限编辑模态框
const showPermissionEditModal = (folder) => {
  editingFolder.value = folder
  
  // 获取当前用户的权限设置
  const existingPermission = folderPermissions.value.find(
    p => p.folderId === folder.id && p.userId === selectedUser.value.id
  )
  
  if (existingPermission) {
    permissionOptions.value = {
      canRead: existingPermission.canRead,
      canUpload: existingPermission.canUpload,
      canDelete: existingPermission.canDelete,
      canCreateFolder: existingPermission.canCreateFolder
    }
  } else {
    permissionOptions.value = {
      canRead: true,
      canUpload: false,
      canDelete: false,
      canCreateFolder: false
    }
  }
  
  showPermissionEditModalFlag.value = true
}

// 关闭权限编辑模态框
const closePermissionEditModal = () => {
  showPermissionEditModalFlag.value = false
  editingFolder.value = null
}

// 保存文件夹权限
const saveFolderPermissions = async () => {
  try {
    const request = {
      folderId: editingFolder.value.id,
      userId: selectedUser.value.id,
      canRead: permissionOptions.value.canRead,
      canUpload: permissionOptions.value.canUpload,
      canDelete: permissionOptions.value.canDelete,
      canCreateFolder: permissionOptions.value.canCreateFolder
    }
    
    // 如果需要创建文件夹权限，则单独调用API
    if (permissionOptions.value.canCreateFolder !== undefined) {
      const createFolderRequest = {
        folderId: editingFolder.value.id,
        userId: selectedUser.value.id,
        canCreateFolder: permissionOptions.value.canCreateFolder
      }
      
      // 先保存普通权限
      await grantFolderPermissionApi(request)
      // 再保存创建文件夹权限
      await grantCreateFolderPermissionApi(createFolderRequest)
    } else {
      // 只保存普通权限
      await grantFolderPermissionApi(request)
    }
    
    showToast(t('admin.permissionSaved'), 'success')
    
    // 更新本地权限数据
    await fetchFolderPermissions(editingFolder.value.id)
    
    closePermissionEditModal()
  } catch (error) {
    console.error('保存权限失败:', error)
    showToast(t('admin.savePermissionFailed'), 'error')
  }
}

// 保存文件夹权限（批量）
const saveFolderPermissionsBatch = async () => {
  try {
    // 收集所有需要保存的权限
    const permissionsToSave = []
    
    // 获取当前用户的所有现有权限（用于比较）
    const existingPermissions = folderPermissions.value.filter(p => p.userId === selectedUser.value.id)
    
    // 收集所有文件夹的权限状态
    const collectPermissions = (nodes) => {
      nodes.forEach(node => {
        // 查找现有的权限记录
        const existingPermission = existingPermissions.find(p => p.folderId === node.id)
        
        if (node.hasPermission) {
          // 如果有权限，准备保存或更新
          permissionsToSave.push({
            folderId: node.id,
            userId: selectedUser.value.id,
            canRead: node.canRead,
            canUpload: node.canUpload,
            canDelete: node.canDelete,
            canCreateFolder: node.canCreateFolder
          })
        } else if (existingPermission) {
          // 如果没有权限但之前有权限，准备删除
          permissionsToSave.push({
            folderId: node.id,
            userId: selectedUser.value.id,
            canRead: false,
            canUpload: false,
            canDelete: false
          })
        }
        
        // 递归处理子节点
        if (node.children && node.children.length > 0) {
          collectPermissions(node.children)
        }
      })
    }
    
    collectPermissions(folderTree.value)
    
    // 使用批量API处理权限变更
    const promises = []
    
    // 批量保存或更新权限
    if (permissionsToSave.length > 0) {
      promises.push(grantFolderPermissionsBatchApi(permissionsToSave))
    }
    
    // 并行执行所有API调用
    if (promises.length > 0) {
      await Promise.all(promises)
    }
    
    showToast(t('admin.permissionSaved'), 'success')
    
    // 重新获取权限信息
    folderPermissions.value = []
    for (const folder of folders.value) {
      await fetchFolderPermissions(folder.id)
    }
    
    // 重新构建带权限的文件夹树
    folderTree.value = buildFolderTreeWithPermissions(
      folders.value, 
      folderPermissions.value, 
      selectedUser.value.id
    )
    
  } catch (error) {
    console.error('保存权限失败:', error)
    showToast(t('admin.savePermissionFailed'), 'error')
  }
}

// 撤销权限
const revokePermission = async () => {
  if (!editingFolder.value) return
  
  try {
    await revokeFolderPermissionApi(editingFolder.value.id, selectedUser.value.id)
    showToast(t('admin.permissionRevoked'), 'success')
    
    // 更新本地权限数据
    await fetchFolderPermissions(editingFolder.value.id)
    
    closePermissionEditModal()
  } catch (error) {
    console.error('撤销权限失败:', error)
    showToast(t('admin.revokePermissionFailed'), 'error')
  }
}

// 获取文件夹权限文本
const getPermissionText = (permission) => {
  const texts = []
  if (permission.canRead) texts.push(t('admin.read'))
  if (permission.canUpload) texts.push(t('admin.upload'))
  if (permission.canDelete) texts.push(t('admin.delete'))
  if (permission.canCreateFolder) texts.push(t('admin.createFolder'))
  return texts.join(', ') || t('admin.noPermission')
}

// 获取指定文件夹的权限
const getFolderPermissions = (folderId) => {
  return folderPermissions.value.filter(
    p => p.folderId === folderId && p.userId === selectedUser.value.id
  )
}

// 检查是否存在现有权限
const hasExistingPermission = computed(() => {
  if (!editingFolder.value) return false
  return folderPermissions.value.some(
    p => p.folderId === editingFolder.value.id && p.userId === selectedUser.value.id
  )
})

// 过滤文件夹列表
const filteredFolders = computed(() => {
  if (!folderSearchTerm.value) return folders.value
  const term = folderSearchTerm.value.toLowerCase()
  return folders.value.filter(folder => 
    folder.name.toLowerCase().includes(term)
  )
})

// 获取用户角色文本
const getUserRoleText = (role) => {
  switch (role) {
    case 'Admin':
      return t('admin.adminUser')
    case 'Normal':
      return t('admin.normalUser')
    default:
      return role
  }
}

// 获取用户状态文本
const getUserStatusText = (status) => {
  switch (status) {
    case 'Active':
      return t('admin.active')
    case 'Inactive':
      return t('admin.inactive')
    case 'Locked':
      return t('admin.locked')
    default:
      return status
  }
}

// 显示 Toast 提示
const showToast = (message, type = 'info') => {
  toastMessage.value = message
  toastType.value = type
}

// 构建文件夹树状结构
const buildFolderTree = (folders, parentId = null) => {
  return folders
    .filter(folder => folder.parentId === parentId)
    .map(folder => {
      const children = buildFolderTree(folders, folder.id)
      return {
        ...folder,
        children,
        expanded: false, // 默认折叠
        indeterminate: false // 半选状态
      }
    })
}

// 构建带权限信息的文件夹树
const buildFolderTreeWithPermissions = (folders, permissions, userId) => {
  const tree = buildFolderTree(folders)
  
  // 递归设置权限状态
  const setPermissions = (nodes) => {
    return nodes.map(node => {
      // 查找用户对该文件夹的权限
      const permission = permissions.find(p => 
        p.folderId === node.id && p.userId === userId
      )
      
      // 设置权限状态
      const canRead = permission ? permission.canRead : false
      const canUpload = permission ? permission.canUpload : false
      const canDelete = permission ? permission.canDelete : false
      const canCreateFolder = permission ? permission.canCreateFolder : false
      
      // 检查是否具有任何权限
      const hasPermission = canRead || canUpload || canDelete || canCreateFolder
      
      return {
        ...node,
        permission: permission || null,
        canRead,
        canUpload,
        canDelete,
        canCreateFolder,
        hasPermission,
        children: setPermissions(node.children || [])
      }
    })
  }
  
  const treeWithPermissions = setPermissions(tree)
  
  // 保持原有的展开状态
  const preserveExpandedState = (newNodes, oldNodes) => {
    if (!oldNodes || oldNodes.length === 0) return newNodes
    
    return newNodes.map((newNode) => {
      // 在旧节点中查找相同ID的节点
      const oldNode = oldNodes.find(n => n.id === newNode.id)
      if (oldNode && oldNode.expanded !== undefined) {
        // 保持原有的展开状态
        newNode.expanded = oldNode.expanded
      }
      
      // 递归处理子节点
      if (newNode.children && newNode.children.length > 0) {
        newNode.children = preserveExpandedState(newNode.children, oldNode?.children || [])
      }
      
      return newNode
    })
  }
  
  return preserveExpandedState(treeWithPermissions, folderTree.value)
}

// 递归渲染文件夹树节点
const renderFolderTreeNode = (folder, level = 0) => {
  return `
    <div class="folder-tree-node" style="margin-left: ${level * 20}px;">
      <div class="folder-tree-item">
        <div class="folder-tree-header">
          <div class="folder-tree-checkbox">
            <input 
              type="checkbox" 
              :checked="folder.hasPermission" 
              @change="toggleFolderPermission(folder)"
            />
          </div>
          <div class="folder-tree-name" @click="toggleFolder(folder)">
            ${folder.children && folder.children.length > 0 ? 
              `<span class="folder-toggle">${folder.expanded ? '▼' : '▶'}</span>` : 
              '<span class="folder-toggle-placeholder"></span>'
            }
            <span class="folder-name-text">${folder.name}</span>
            ${folder.isPublic ? `<span class="public-badge">${t('admin.publicFolder')}</span>` : ''}
          </div>
        </div>
        ${folder.hasPermission ? `
          <div class="folder-permissions-detail">
            <label class="permission-checkbox">
              <input type="checkbox" v-model="folder.canRead" />
              ${t('admin.canRead')}
            </label>
            <label class="permission-checkbox">
              <input type="checkbox" v-model="folder.canUpload" />
              ${t('admin.canUpload')}
            </label>
            <label class="permission-checkbox">
              <input type="checkbox" v-model="folder.canDelete" />
              ${t('admin.canDelete')}
            </label>
          </div>
        ` : ''}
      </div>
      ${folder.expanded && folder.children && folder.children.length > 0 ? `
        <div class="folder-tree-children">
          ${folder.children.map(child => renderFolderTreeNode(child, level + 1)).join('')}
        </div>
      ` : ''}
    </div>
  `
}

// 切换文件夹展开/折叠状态
const toggleFolder = (folder) => {
  // 递归查找并更新文件夹的展开状态
  const updateFolderExpanded = (nodes) => {
    return nodes.map(node => {
      if (node.id === folder.id) {
        // 创建新对象以确保响应式更新
        return { ...node, expanded: !node.expanded };
      }
      // 递归处理子节点
      if (node.children && node.children.length > 0) {
        return { ...node, children: updateFolderExpanded(node.children) };
      }
      return node;
    });
  };
  
  // 更新整个文件夹树
  folderTree.value = updateFolderExpanded(folderTree.value);
}

// 切换文件夹权限选择
const toggleFolderPermission = (folder) => {
  // 如果当前没有权限，则设置默认权限（只读）
  if (!folder.hasPermission) {
    folder.canRead = true
    folder.canUpload = false
    folder.canDelete = false
    folder.hasPermission = true
    
    // 联动选择所有上级文件夹
    selectParentFolders(folder);
  } else {
    // 如果已有权限，则清除权限
    folder.canRead = false
    folder.canUpload = false
    folder.canDelete = false
    folder.hasPermission = false
  }
  
  // 更新子文件夹权限
  const updateChildren = (children, hasPermission, canRead, canUpload, canDelete) => {
    children.forEach(child => {
      child.hasPermission = hasPermission
      child.canRead = canRead
      child.canUpload = canUpload
      child.canDelete = canDelete
      if (child.children && child.children.length > 0) {
        updateChildren(child.children, hasPermission, canRead, canUpload, canDelete)
      }
    })
  }
  
  if (folder.children && folder.children.length > 0) {
    updateChildren(
      folder.children, 
      folder.hasPermission, 
      folder.canRead, 
      folder.canUpload, 
      folder.canDelete
    )
  }
}

// 联动选择所有上级文件夹
const selectParentFolders = (folder) => {
  // 创建文件夹ID到文件夹对象的映射，提高查找效率
  const folderMap = new Map();
  
  // 递归遍历文件夹树，建立映射关系
  const buildFolderMap = (nodes) => {
    nodes.forEach(node => {
      folderMap.set(node.id, node);
      if (node.children && node.children.length > 0) {
        buildFolderMap(node.children);
      }
    });
  };
  
  buildFolderMap(folderTree.value);
  
  // 从当前文件夹开始，逐级向上选择父文件夹
  let currentFolder = folder;
  while (currentFolder.parentId) {
    const parentFolder = folderMap.get(currentFolder.parentId);
    if (parentFolder && !parentFolder.hasPermission) {
      parentFolder.canRead = true;
      parentFolder.canUpload = false;
      parentFolder.canDelete = false;
      parentFolder.hasPermission = true;
    }
    currentFolder = parentFolder;
  }
}

// 撤销所有权限
const revokeAllPermissions = async () => {
  if (!confirm(t('admin.confirmRevokeAllPermissions', { username: selectedUser.value.username }))) {
    return
  }
  
  try {
    // 收集所有需要撤销的权限
    const permissionsToRevoke = []
    
    const collectPermissions = (nodes) => {
      nodes.forEach(node => {
        if (node.hasPermission) {
          permissionsToRevoke.push({
            folderId: node.id,
            userId: selectedUser.value.id
          })
        }
        if (node.children && node.children.length > 0) {
          collectPermissions(node.children)
        }
      })
    }
    
    collectPermissions(folderTree.value)
    
    // 使用批量API撤销权限
    if (permissionsToRevoke.length > 0) {
      await revokeFolderPermissionsBatchApi(permissionsToRevoke)
    }
    
    showToast(t('admin.permissionRevoked'), 'success')
    
    // 重新获取权限信息
    folderPermissions.value = []
    for (const folder of folders.value) {
      await fetchFolderPermissions(folder.id)
    }
    
    // 重新构建带权限的文件夹树
    folderTree.value = buildFolderTreeWithPermissions(
      folders.value, 
      folderPermissions.value, 
      selectedUser.value.id
    )
    
  } catch (error) {
    console.error('撤销权限失败:', error)
    showToast(t('admin.revokePermissionFailed'), 'error')
  }
}

</script>

<style scoped>
.user-management-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

.title {
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-bottom: 30px;
  text-align: center;
}

.toolbar {
  margin-bottom: 20px;
  text-align: right;
}

.create-button {
  padding: 10px 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  box-shadow: 0 2px 10px rgba(102, 126, 234, 0.3);
}

.create-button:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

/* 桌面端表格视图 */
.desktop-table {
  background-color: white;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  overflow-x: auto; /* 允许水平滚动 */
}

.desktop-table table {
  width: 100%;
  border-collapse: collapse;
}

.desktop-table th,
.desktop-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
  white-space: nowrap;
}

.desktop-table th {
  background-color: #f8f9fa;
  font-weight: bold;
  color: #555;
}

.desktop-table tbody tr:hover {
  background-color: #f8f9fa;
}

.desktop-table .action-button {
  padding: 6px 10px;
  margin: 0 2px;
  font-size: 13px;
}

.permission-button {
  background-color: #28a745;
  color: white;
}

/* 移动端卡片视图 */
.mobile-users-list {
  display: none;
}

.user-card {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.user-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.user-card-header h3 {
  margin: 0;
  font-size: 18px;
  color: #333;
}

.user-id {
  background-color: #e9ecef;
  color: #495057;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
}

.user-card-body {
  margin-bottom: 15px;
}

.user-info {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  flex-wrap: wrap;
}

.user-info:last-child {
  margin-bottom: 0;
}

.info-label {
  font-weight: bold;
  color: #555;
  flex: 1;
  min-width: 80px;
}

.info-value {
  color: #333;
  text-align: right;
  flex: 2;
  word-break: break-word;
}

.user-card-actions {
  display: flex;
  justify-content: space-between;
  gap: 10px;
  flex-wrap: wrap;
}

.action-button {
  padding: 8px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  flex: 1;
  margin: 0 5px;
  white-space: nowrap;
  min-width: 80px;
}

.edit-button {
  background-color: #17a2b8;
  color: white;
}

.permission-button {
  background-color: #28a745;
  color: white;
}

.reset-password-button {
  background-color: #ffc107;
  color: #212529;
}

.delete-button {
  background-color: #dc3545;
  color: white;
}

.action-button:hover {
  opacity: 0.8;
}

/* 模态框样式 */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.permission-modal .modal-content {
  max-width: 800px;
  width: 90%;
}

.edit-permission-modal .modal-content {
  max-width: 500px;
  width: 90%;
}

.modal-content h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #555;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  box-sizing: border-box;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  margin-top: 30px;
}

.save-button,
.cancel-button,
.delete-button {
  padding: 12px 24px;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  flex: 1;
  margin: 0 10px;
}

.save-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.cancel-button {
  background-color: #6c757d;
  color: white;
}

.delete-button {
  background-color: #dc3545;
  color: white;
}

.save-button:hover,
.cancel-button:hover,
.delete-button:hover {
  opacity: 0.9;
}

/* 权限管理模态框样式 */
.permission-content {
  margin-bottom: 20px;
}

.folder-list {
  margin-bottom: 20px;
}

.folder-search {
  margin-bottom: 15px;
}

.search-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  box-sizing: border-box;
}

.folder-items {
  max-height: 400px;
  overflow-y: auto;
}

.folder-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #eee;
}

.folder-item:last-child {
  border-bottom: none;
}

.folder-info {
  flex: 1;
}

.folder-name {
  font-weight: bold;
  margin-right: 10px;
}

.public-badge {
  background-color: #28a745;
  color: white;
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 12px;
}

.folder-permissions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.permission-tag {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: bold;
}

.permission-tag.read {
  background-color: #17a2b8;
  color: white;
}

.permission-tag.upload {
  background-color: #28a745;
  color: white;
}

.permission-tag.delete {
  background-color: #dc3545;
  color: white;
}

.permission-tag.create-folder {
  background-color: #6f42c1;
  color: white;
}

.edit-permission-button {
  padding: 6px 12px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.edit-permission-button:hover {
  background-color: #0056b3;
}

.permission-options {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: normal;
  cursor: pointer;
}

.checkbox-label input {
  width: auto;
  margin: 0;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .user-management-container {
    padding: 15px;
    overflow-x: hidden; /* 防止水平滚动 */
  }
  
  .title {
    font-size: 22px;
  }
  
  .toolbar {
    text-align: center;
  }
  
  .create-button {
    width: 100%;
    padding: 12px;
    font-size: 16px;
  }
  
  /* 在移动端隐藏桌面表格 */
  .desktop-table {
    display: none;
  }
  
  /* 在移动端显示卡片列表 */
  .mobile-users-list {
    display: block;
    width: 100%;
    overflow-x: hidden; /* 防止水平滚动 */
  }
  
  .user-card-actions {
    flex-direction: column;
  }
  
  .action-button {
    margin: 5px 0;
    width: 100%;
  }
  
  .modal-content {
    padding: 20px;
    width: 95%;
    margin: 20px;
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .save-button,
  .cancel-button {
    margin: 5px 0;
  }
  
  .user-info {
    flex-direction: column;
    gap: 3px;
  }
  
  .info-label,
  .info-value {
    text-align: left;
    width: 100%;
  }
  
  .folder-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .folder-permissions {
    width: 100%;
    justify-content: space-between;
  }
  
  .permission-modal .modal-content {
    padding: 15px;
  }
  
  .folder-tree-container {
    max-height: 300px;
    overflow-y: auto;
  }
  
  .folder-search .search-input {
    padding: 8px;
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .user-management-container {
    padding: 10px;
  }
  
  .title {
    font-size: 20px;
  }
  
  .user-card {
    padding: 15px;
    margin-bottom: 15px;
  }
  
  .user-card-header h3 {
    font-size: 16px;
  }
  
  .info-label,
  .info-value {
    font-size: 14px;
  }
  
  .action-button {
    padding: 6px 10px;
    font-size: 13px;
    min-width: 70px;
  }
  
  /* 超小屏幕设备的额外优化 */
  .user-card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 5px;
  }
  
  .user-id {
    align-self: flex-end;
  }
  
  .folder-permissions {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .modal-content {
    padding: 15px;
    margin: 10px;
  }
  
  .modal-content h2 {
    font-size: 18px;
  }
  
  .form-group input,
  .form-group select {
    padding: 8px;
    font-size: 14px;
  }
  
  .save-button,
  .cancel-button,
  .delete-button {
    padding: 10px 16px;
    font-size: 14px;
  }
  
  .folder-tree-actions {
    flex-direction: column;
    gap: 10px;
  }
  
  .folder-tree-actions .action-button {
    width: 100%;
    margin: 0;
  }
}
</style>