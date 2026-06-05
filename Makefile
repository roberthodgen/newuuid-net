PROJECT ?= src/NewUuidNet/NewUuidNet.csproj
PROJECT_LOCATION ?= src/NewUuidNet
CONFIGURATION ?= Release
FRAMEWORK ?= net8.0
BUILD_DIR ?= build
PACKAGE_FILE ?= $(BUILD_DIR)/newuuid-net.zip

AWS_PROFILE ?= default
AWS_REGION ?= us-east-1
STACK_NAME ?= newuuid-net
S3_BUCKET ?= newuuid-net

.PHONY: build deploy clean

build:
	@mkdir -p "$(BUILD_DIR)"
	@rm -f "$(PACKAGE_FILE)"
	dotnet lambda package \
		--project-location "$(PROJECT_LOCATION)" \
		--configuration "$(CONFIGURATION)" \
		--framework "$(FRAMEWORK)" \
		--output-package "$(PACKAGE_FILE)"

deploy: build
	dotnet lambda deploy-serverless \
		--disable-interactive true \
		--project-location "$(PROJECT_LOCATION)" \
		--profile "$(AWS_PROFILE)" \
		--region "$(AWS_REGION)" \
		--stack-name "$(STACK_NAME)" \
		--s3-bucket "$(S3_BUCKET)" \
		--package "$(PACKAGE_FILE)"

clean:
	@rm -rf "$(BUILD_DIR)"
	@dotnet clean "$(PROJECT)" --configuration "$(CONFIGURATION)"
