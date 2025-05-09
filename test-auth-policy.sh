#!/bin/bash

# 设置基本变量
API_URL="http://localhost:5022/api"
USERNAME="admin"
PASSWORD="Admin@123456"
ACCESS_TOKEN=""

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
NC='\033[0m' # No Color

# 显示标题
echo -e "${YELLOW}=== 授权测试脚本 ===${NC}"
echo "测试各种API端点的授权策略"
echo ""

# 登录并获取令牌
echo -e "${YELLOW}1. 登录获取令牌${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "${API_URL}/Auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"${USERNAME}\",\"password\":\"${PASSWORD}\"}")

# 检查登录是否成功
if [[ $LOGIN_RESPONSE == *"accessToken"* ]]; then
  echo -e "${GREEN}登录成功!${NC}"
  # 提取访问令牌
  ACCESS_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | sed 's/"accessToken":"//g')
  echo "获取到令牌: ${ACCESS_TOKEN:0:20}..." # 只显示令牌的前20个字符
  
  # 解码JWT令牌并输出其内容（不验证签名）
  echo -e "\n${YELLOW}JWT令牌内容:${NC}"
  JWT_HEADER=$(echo $ACCESS_TOKEN | cut -d. -f1 | base64 -d 2>/dev/null || echo "无法解析JWT头部")
  JWT_PAYLOAD=$(echo $ACCESS_TOKEN | cut -d. -f2 | base64 -d 2>/dev/null || echo "无法解析JWT载荷")
  echo "头部: $JWT_HEADER"
  echo "载荷: $JWT_PAYLOAD"
  
  # 显示用户的角色和权限
  echo -e "\n${YELLOW}用户角色和权限:${NC}"
  ROLES=$(echo $JWT_PAYLOAD | grep -o '"role":"[^"]*' | sed 's/"role":"//g' | sort | uniq)
  PERMISSIONS=$(echo $JWT_PAYLOAD | grep -o '"permission":"[^"]*' | sed 's/"permission":"//g' | sort | uniq)
  echo "角色: $ROLES"
  echo "权限: $PERMISSIONS"
  
  # 验证不同API端点的授权
  echo -e "\n${YELLOW}2. 测试需要管理员角色的端点${NC}"
  
  # 测试创建角色API (需要RequireAdminRole策略)
  echo "测试创建角色 (RequireAdminRole):"
  ROLE_API_RESPONSE=$(curl -s -w "\nHTTP状态码: %{http_code}" -X POST "${API_URL}/applications/1/roles" \
    -H "Authorization: Bearer ${ACCESS_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{"name":"test-role","description":"测试角色"}')
  echo -e "$ROLE_API_RESPONSE"
  
  # 测试获取角色列表API (需要RequirePermission策略)
  echo -e "\n${YELLOW}3. 测试需要权限的端点${NC}"
  echo "测试获取角色列表 (RequirePermission):"
  ROLES_API_RESPONSE=$(curl -s -w "\nHTTP状态码: %{http_code}" -X GET "${API_URL}/applications/1/roles" \
    -H "Authorization: Bearer ${ACCESS_TOKEN}")
  echo -e "$ROLES_API_RESPONSE"
  
  # 测试获取权限列表API (需要RequirePermission策略)
  echo -e "\n测试获取权限列表 (RequirePermission):"
  PERMISSIONS_API_RESPONSE=$(curl -s -w "\nHTTP状态码: %{http_code}" -X GET "${API_URL}/applications/1/permissions" \
    -H "Authorization: Bearer ${ACCESS_TOKEN}")
  echo -e "$PERMISSIONS_API_RESPONSE"
  
  # 测试获取用户列表API (需要RequirePermission策略)
  echo -e "\n测试获取用户列表 (RequirePermission):"
  USERS_API_RESPONSE=$(curl -s -w "\nHTTP状态码: %{http_code}" -X GET "${API_URL}/applications/1/users" \
    -H "Authorization: Bearer ${ACCESS_TOKEN}")
  echo -e "$USERS_API_RESPONSE"
  
  # 测试用户资料API (需要基本授权)
  echo -e "\n${YELLOW}4. 测试只需基本授权的端点${NC}"
  echo "测试获取个人资料 (基本授权):"
  PROFILE_API_RESPONSE=$(curl -s -w "\nHTTP状态码: %{http_code}" -X GET "${API_URL}/Auth/profile" \
    -H "Authorization: Bearer ${ACCESS_TOKEN}")
  echo -e "$PROFILE_API_RESPONSE"
  
else
  echo -e "${RED}登录失败!${NC}"
  echo "错误信息: $LOGIN_RESPONSE"
  exit 1
fi

echo -e "\n${YELLOW}=== 测试完成 ===${NC}"
