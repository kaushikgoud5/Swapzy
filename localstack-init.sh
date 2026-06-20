#!/bin/bash
awslocal s3 mb s3://swapzy-product-images
awslocal sns create-topic --name swapzy-events
awslocal sqs create-queue --queue-name swapzy-notifications
awslocal sns subscribe \
  --topic-arn arn:aws:sns:ap-south-1:000000000000:swapzy-events \
  --protocol sqs \
  --notification-endpoint arn:aws:sqs:ap-south-1:000000000000:swapzy-notifications
