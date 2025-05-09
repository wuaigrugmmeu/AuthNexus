#!/bin/bash

# 测试变量
API_URL="http://localhost:5022/api"
TEST_USERNAME="admin"
TEST_PASSWORD="Admin@123456"

# 颜色定义
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}开始测试登录和获取个人资料功能...${NC}"

# 测试登录API
echo "1. 测试登录API..."
LOGIN_RESPONSE=$(curl -s -X POST $API_URL/Auth/login \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$TEST_USERNAME\",\"password\":\"$TEST_PASSWORD\"}")

# 检查是否包含 accessToken
if [[ $LOGIN_RESPONSE == *"accessToken"* ]]; then
  echo -e "${GREEN}登录成功!${NC}"
  
  # 提取 accessToken
  ACCESS_TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | sed 's/"accessToken":"//')
  USER_ID=$(echo $LOGIN_RESPONSE | grep -o '"userId":"[^"]*' | sed 's/"userId":"//')
  
  echo "获取到的访问令牌: ${ACCESS_TOKEN:0:20}..."
  echo "获取到的用户ID: $USER_ID"
  
  # 测试获取个人资料API
  echo -e "\n2. 测试获取个人资料API..."
  PROFILE_RESPONSE=$(curl -s -X GET $API_URL/Auth/profile \
    -H "Authorization: Bearer $ACCESS_TOKEN")
  
  # 检查是否成功获取个人资料
  if [[ $PROFILE_RESPONSE == *"username"* ]]; then
    echo -e "${GREEN}获取个人资料成功!${NC}"
    echo "个人资料数据: $PROFILE_RESPONSE"
  else
    echo -e "${RED}获取个人资料失败!${NC}"
    echo "错误信息: $PROFILE_RESPONSE"
  fi
else
  echo -e "${RED}登录失败!${NC}"
  echo "错误信息: $LOGIN_RESPONSE"
fi
