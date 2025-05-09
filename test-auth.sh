#!/bin/bash

# 测试变量
API_URL="http://localhost:5022/api"
TEST_USERNAME="admin"
TEST_PASSWORD="Admin@123456"

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 创建测试结果目录
RESULTS_DIR="auth_test_results"
mkdir -p $RESULTS_DIR

# 测试端点函数
test_endpoint() {
  local method=$1
  local endpoint=$2
  local description=$3
  local data=$4
  
  echo -e "\n${BLUE}测试: $description${NC}"
  
  local curl_cmd="curl -s -X $method $API_URL$endpoint -H 'Authorization: Bearer $ACCESS_TOKEN' -H 'Content-Type: application/json' -w '\nSTATUS_CODE:%{http_code}'"
  
  [ ! -z "$data" ] && curl_cmd="$curl_cmd -d '$data'"
  
  local RESPONSE=$(eval $curl_cmd)
  local STATUS_CODE=$(echo "$RESPONSE" | grep -o 'STATUS_CODE:[0-9]*' | cut -d':' -f2)
  local RESPONSE_BODY=$(echo "$RESPONSE" | sed 's/STATUS_CODE:[0-9]*$//')
  
  if [[ "$STATUS_CODE" =~ ^(200|201|204)$ ]]; then
    echo -e "${GREEN}✓ 成功 ($STATUS_CODE)${NC}"
    return 0
  else
    echo -e "${RED}✗ 失败 ($STATUS_CODE)${NC}"
    return 1
  fi
}

# 解析JWT令牌
parse_jwt() {
  local token=$1
  local payload=$(echo -n $token | cut -d "." -f2 | base64 -d 2>/dev/null || echo -n $token | cut -d "." -f2 | base64 --decode 2>/dev/null)
  
  echo -e "\n${BLUE}JWT令牌信息:${NC}"
  
  # 提取角色和权限
  local roles=$(echo $payload | grep -o '"role":"[^"]*"' | sed 's/"role":"//g' | sed 's/"//g' | sort | uniq)
  local permissions=$(echo $payload | grep -o '"permission":"[^"]*"' | sed 's/"permission":"//g' | sed 's/"//g' | sort | uniq)
  
  echo -e "${YELLOW}角色:${NC} $(echo $roles | tr '\n' ' ')"
  echo -e "${YELLOW}权限:${NC} $(echo $permissions | tr '\n' ' ')"
}

echo -e "${YELLOW}===== AuthNexus API 测试 =====${NC}"

# 登录测试
echo -e "\n${BLUE}登录测试...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST $API_URL/Auth/login \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$TEST_USERNAME\",\"password\":\"$TEST_PASSWORD\"}")

if [[ $LOGIN_RESPONSE == *"accessToken"* ]]; then
  echo -e "${GREEN}✓ 登录成功${NC}"
  
  # 提取令牌
  ACCESS_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | sed 's/"accessToken":"//')
  USER_ID=$(echo $LOGIN_RESPONSE | grep -o '"userId":"[^"]*' | sed 's/"userId":"//')
  
  # 解析JWT
  parse_jwt "$ACCESS_TOKEN"
  
  # 测试个人资料
  test_endpoint "GET" "/Auth/profile" "获取个人资料"
  
  # 获取应用程序信息
  APP_LIST_RESPONSE=$(curl -s -X GET $API_URL/Applications -H "Authorization: Bearer $ACCESS_TOKEN")
  APP_ID=$(echo $APP_LIST_RESPONSE | grep -o '"id":"[^"]*' | head -1 | sed 's/"id":"//')
  APP_UID=$(echo $APP_LIST_RESPONSE | grep -o '"appUID":"[^"]*' | head -1 | sed 's/"appUID":"//')
  [ -z "$APP_ID" ] && APP_ID="9F4696D3-FE38-4D99-A100-B8DE7402CB79"  # 使用实际存在的默认ID
  [ -z "$APP_UID" ] && APP_UID="admin-app-001"  # 使用实际存在的默认AppUID
  
  echo -e "\n找到应用程序ID: $APP_ID, AppUID: $APP_UID，用于后续测试"
  
  echo -e "\n${YELLOW}基本API测试:${NC}"
  # 应用程序管理
  test_endpoint "GET" "/Applications" "获取所有应用程序"
  test_endpoint "GET" "/Applications/$APP_ID" "获取特定应用程序"
  
  # 角色管理
  test_endpoint "GET" "/Applications/$APP_UID/Roles" "获取角色"
  NEW_ROLE_NAME="TestRole_$(date +%s)"
  test_endpoint "POST" "/Applications/$APP_UID/Roles" "创建新角色" \
    "{\"applicationId\":\"$APP_UID\",\"name\":\"$NEW_ROLE_NAME\",\"description\":\"测试角色\"}"
  
  # 权限管理
  test_endpoint "GET" "/Applications/$APP_UID/Permissions" "获取权限"
  NEW_PERM_CODE="test:perm:$(date +%s)"
  test_endpoint "POST" "/Applications/$APP_UID/Permissions" "创建新权限" \
    "{\"applicationId\":\"$APP_UID\",\"code\":\"$NEW_PERM_CODE\",\"description\":\"测试权限\"}"
  
  # 用户管理
  test_endpoint "GET" "/Applications/$APP_UID/Users" "获取所有用户"
  test_endpoint "GET" "/Applications/$APP_UID/Users/$USER_ID/Permissions" "获取用户权限"
  
  echo -e "\n${YELLOW}权限检查:${NC}"
  # 检查几个关键权限
  PERMISSIONS=("users:view" "roles:view" "permissions:view" "applications:view")
  
  for perm in "${PERMISSIONS[@]}"; do
    RESP=$(curl -s -X GET "$API_URL/Diagnostics/check-permission/$perm" -H "Authorization: Bearer $ACCESS_TOKEN")
    HAS_PERM=$(echo "$RESP" | grep -o '"hasPermission":[^,}]*' | cut -d':' -f2)
    
    if [ "$HAS_PERM" = "true" ]; then
      echo -e "${GREEN}✓ 具有权限: $perm${NC}"
    else
      echo -e "${RED}✗ 缺少权限: $perm${NC}"
    fi
  done
else
  echo -e "${RED}✗ 登录失败${NC}"
  echo "$LOGIN_RESPONSE"
fi

echo -e "\n${YELLOW}===== 测试完成 =====${NC}"
