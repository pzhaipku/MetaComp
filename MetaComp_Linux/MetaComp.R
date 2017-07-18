readFeature<-function(file, featureType="NA",format){
  if(format=="txt")
  {
		# Read feature file
		data<-read.table(file, header=TRUE, sep="\t")
  }
  else if(format=="biom_table")
  {
  	data<-read.table(file, sep="\t", skip=2)
  	colnames(data)<-data[1,]
  	data<-data[-1,]
  }
	#featureNames<-colnames(data)
	ncols<-dim(data)[2]
	nrows<-dim(data)[1]
  
  # Create the data struct
	featureData<-list()
	featureData$CountMatrix<-(data[,2:ncols])
  featureData$Type<-featureType
 	featureData$FeaName<-data[1:nrows,1]
	featureData$SampName<-colnames(data)[-1]
  
	# Convert to frequency matrix
	featureData$FreqMatrix <- sweep(featureData$CountMatrix,2,colSums(featureData$CountMatrix),FUN="/")

	# set names for data matrix
	rownames(featureData$CountMatrix)=featureData$FeaName
	colnames(featureData$CountMatrix)=featureData$SampName

	rownames(featureData$FreqMatrix)=featureData$FeaName
	colnames(featureData$FreqMatrix)=featureData$SampName


	# get annotation
	featureData$Annotation <- getAnnotation(featureData$FeaName,featureType=tolower(featureType))
    
	return(featureData)
}

getAnnotation <- function(featurelist, featureType)
{
	load("Annotation.RData")
	supportlist <- c("cog","pfam")
	num <- length(featurelist)
	annolist <- c("tmp",num)
	if ( sum(featureType == supportlist) == 0  )
	{
		return(rep(NA, length(featurelist)))
	}
	for (i in 1:num)
	{	
		row <- which(AnnoData[[featureType]][,1]==featurelist[i])
		annolist[i] = AnnoData[[featureType]][row,2]
	}
	 
	return(annolist)
}

twoSamplesComp<-function(featureData, alpha = 0.05, resultFileName = "result.csv", isSilent = FALSE, sample1 = 1, sample2 = 2 )
{
	# compute d-score
	sample_total <- colSums( featureData$CountMatrix )
	f1 <- featureData$CountMatrix[,sample1]/sample_total[sample1]
	f2 <- featureData$CountMatrix[,sample2]/sample_total[sample2]
	prob <- (featureData$CountMatrix[,sample1] + featureData$CountMatrix[,sample2])/(sample_total[sample1] + sample_total[sample2])
	stat <- (f1-f2)/sqrt(prob*(1-prob)*(1/sample_total[sample1]+1/sample_total[sample2]))
	stat[is.nan(stat)] = 0

	# p-value
	p.value <- 2*(pnorm(-abs(stat)))

	# put p=NA for low numbers
	leastnum = ceiling ( qnorm(alpha/2)^2 )
	tmpmatrix <- cbind( featureData$CountMatrix[,sample1], featureData$CountMatrix[,sample2] )
	p.value[rowSums(tmpmatrix < leastnum)==2] = NA

	bonferroni.p <- p.adjust(p.value,"bonferroni")
	q.value <- p.adjust(p.value,"fdr")



	result <- data.frame(featureData$FeaName, featureData$CountMatrix[,sample1], featureData$CountMatrix[,sample2], 
			p.value, q.value, bonferroni.p,featureData$Annotation)
	colnames( result ) <- c("Feature", featureData$SampName[sample1], featureData$SampName[sample2], "p.value","q.value","bonferroni.p", "Annotation")
	result <- result [order(result[,"p.value"]),]

	if ( !isSilent )
	{
		sig.p <- length(result[t(which(result[,"p.value"]<alpha)),1])
		sig.q <- length(result[t(which(result[,"q.value"]<alpha)),1])
		sig.bonferroni <- length(result[t(which(result[,"bonferroni.p"]<alpha)),1])

		cat("Two samples test: ", featureData$SampName[sample1], " vs ", featureData$SampName[sample2], "\n")
		cat("Feature number: ", length(featureData$FeaName), "\n")
		cat("p.value <",alpha," : ",sig.p,"\tq.value <",alpha," : ",sig.q,"\tbonferroni.p <",alpha," : ",sig.bonferroni,"\n")
		cat("Result details are saved in : ", resultFileName, "\n")
		write.csv(result,resultFileName)
	}

	return(result)
}


multipleSamplesComp<-function(featureData, alpha = 0.05, isSilent = FALSE, resultFileName = "result.csv" )
{
	sampleNum=length(featureData$SampName)
	feaNum=length(featureData$FeaName)
	min_p <- rep(NA,times=feaNum)
	min_fdr_p <- rep(NA,times=feaNum)
	min_bonferroni_p <- rep(NA,times=feaNum)
	diff <- rep(NA,times=feaNum)

	for (sample1 in 1:(sampleNum-1))
	{
		for ( sample2 in (sample1+1):sampleNum)
		{
			tmp <- twoSamplesComp(featureData, sample1 = sample1, sample2 = sample2, alpha = alpha, isSilent = TRUE)
			order_tmp <- tmp[order(tmp[,1]),]
			for ( j in 1:feaNum )
			{ 
				if ( !is.na(order_tmp[j,"p.value"]) )
				{ 
					if ( is.na(min_p[j]) | order_tmp[j,"p.value"] < min_p[j] )
					{
						min_p[j] = order_tmp[j,"p.value"]
						min_fdr_p[j] = order_tmp[j,"q.value"]
						min_bonferroni_p[j] = order_tmp[j,"bonferroni.p"]
						diff[j] = paste(sample1,sample2,sep=",")
					}
				}
			}
		}
	}
	result <- data.frame(featureData$CountMatrix,min_p,min_fdr_p,min_bonferroni_p,featureData$Annotation)
	colnames( result ) <- c(featureData$SampName, "p.value","q.value","bonferroni.p", "Annotation")

	result <- result [order(result[,"p.value"]),]
	if ( !isSilent )
	{
		sig.p <- length(result[t(which(result[,"p.value"]<alpha)),1])
		sig.q <- length(result[t(which(result[,"q.value"]<alpha)),1])
		sig.bonferroni <- length(result[t(which(result[,"bonferroni.p"]<alpha)),1])

		cat("Multiple samples test:\n")
		cat("Feature number: ", feaNum, "\n")
		cat("p.value <",alpha," : ",sig.p,"\tq.value <",alpha," : ",sig.q,"\tbonferroni.p <",alpha," : ",sig.bonferroni,"\n")
		cat("Result details are saved in : ", resultFileName, "\n")
		write.csv(result,resultFileName)
	}
	
	return(result)
}

twoGroupsComp<-function(featureData, groupsep, alpha = 0.05, resultFileName = "result.csv",
method = c("t-test","paired-t-test","w-rank-sum","w-signed-rank"), 
group1 = "Group1", group2 = "Group2" )
{

	method = match.arg(method)

	sampleNum=length(featureData$SampName)
	feaNum=length(featureData$FeaName)
	samp1_mean <- apply(featureData$FreqMatrix[,1:groupsep],1,mean)
	samp2_mean <- apply(featureData$FreqMatrix[,(groupsep+1):sampleNum],1,mean)

	if ( method=="t-test" )
	{	
		samp1_sd <- apply(featureData$FreqMatrix[,1:groupsep],1,sd)
		samp2_sd <- apply(featureData$FreqMatrix[,(groupsep+1):sampleNum],1,sd)
		
		a <- samp1_sd^2/(groupsep)
		b <- samp2_sd^2/(sampleNum-groupsep)
		t <- (samp1_mean-samp2_mean)/sqrt( a + b )
		df <- round( (a+b)^2 / ( a^2/(groupsep) + b^2/(sampleNum-groupsep) ) )	
		p.value <- 2*(pt( -abs(t), df ))
	}

	else if (method=="paired-t-test")
	{
		if ( (groupsep) != sampleNum-groupsep )
		{
			cat ("paired test must have same number samples in each category")
			return ("paired test must have same number samples in each category")

		}
		p.value <- rep(NA,times=feaNum)
		for ( j in 1:feaNum)
		{
			group1_freq <- as.numeric(featureData$FreqMatrix[j,1:groupsep])
			group2_freq <- as.numeric(featureData$FreqMatrix[j,(groupsep+1):sampleNum])
			p.value[j] = t.test(group1_freq,group2_freq, paired=TRUE)$p.value
		}
	}

	else if (method=="w-rank-sum")
	{
		p.value <- rep(NA,times=feaNum)
		for ( j in 1:feaNum)
		{
			group1_freq <- as.numeric(featureData$FreqMatrix[j,1:groupsep])
			group2_freq <- as.numeric(featureData$FreqMatrix[j,(groupsep+1):sampleNum])
			p.value[j] = wilcox.test(group1_freq,group2_freq,exact = FALSE)$p.value
		}
	}

	else if (method=="w-signed-rank")
	{
		if ( (groupsep) != sampleNum-groupsep )
		{
			cat ("paired test must have same number samples in each category")
			return ("paired test must have same number samples in each category")

		}

		p.value <- rep(NA,times=feaNum)
		for ( j in 1:feaNum)
		{
			group1_freq <- as.numeric(featureData$FreqMatrix[j,1:groupsep])
			group2_freq <- as.numeric(featureData$FreqMatrix[j,(groupsep+1):sampleNum])
			p.value[j] = wilcox.test(group1_freq,group2_freq,paired=TRUE,exact = FALSE)$p.value
		}
	}

	data <- featureData$CountMatrix
	bonferroni.p <- p.adjust(p.value,"bonferroni")
	q.value <- p.adjust(p.value,"fdr")

	s1_mean <- apply(featureData$FreqMatrix[,1:groupsep],1,mean)
	s2_mean <- apply(featureData$FreqMatrix[,(groupsep+1):sampleNum],1,mean)
	s1_sd <- apply(featureData$FreqMatrix[,1:groupsep],1,sd)
	s2_sd <- apply(featureData$FreqMatrix[,(groupsep+1):sampleNum],1,sd)

	result <- data.frame(featureData$CountMatrix,p.value, q.value,bonferroni.p,s1_mean,s1_sd,s2_mean,s2_sd, featureData$Annotation)
	colnames( result ) <- c(featureData$SampName, "p.value","q.value","bonferroni.p", group1, ,group2, ,"Annotation")

	result <- result [order(result[,"p.value"]),]

	sig.p <- length(result[t(which(result[,"p.value"]<alpha)),1])
	sig.q <- length(result[t(which(result[,"q.value"]<alpha)),1])
	sig.bonferroni <- length(result[t(which(result[,"bonferroni.p"]<alpha)),1])

	cat("Two Groups of samples test: ", group1, " vs ", group2, "\n")
	cat("Feature number: ", feaNum, "\n")
	cat("p.value <",alpha," : ",sig.p,"\tq.value <",alpha," : ",sig.q,"\tbonferroni.p <",alpha," : ",sig.bonferroni,"\n")
	cat("Result details are saved in : ", resultFileName, "\n")
	write.csv(result,resultFileName)

	return(result)


}

oddRatioTest <- function(featureData, groupsep, resultFileName = "result.csv",
group1 = "Group1", group2 = "Group2")
{
	data <- featureData$CountMatrix
	sampleNum=length(featureData$SampName)
	feaNum=length(featureData$FeaName)
	samp1_mean <- apply(featureData$CountMatrix[,1:groupsep-1],1,mean)
	samp2_mean <- apply(featureData$CountMatrix[,groupsep:sampleNum],1,mean)


	odd_ratio <- rep(0,times=feaNum)
	p.value <- rep(NA,times=feaNum)
	s1 <- sum(sum(data[,1:groupsep-1]))
	s2 <- sum(sum(data[,groupsep:sampleNum]))

	for (j in 1:feaNum)
	{
		feature_sample1 <- sum(data[j,1:groupsep-1])
		feature_sample2 <- sum(data[j,groupsep:sampleNum])

		otherfeature_sample2 <- s2-feature_sample2
		otherfeature_sample1 <- s1-feature_sample1

		R <- s1/s2
		treatadd <- R/(R+1)
		controladd <- 1/(R+1)
		odd_ratio[j] = log2(((feature_sample1+treatadd)*(otherfeature_sample2+controladd))/((feature_sample2+controladd)*(otherfeature_sample1+treatadd )))

	}

	odd_ratio[which(rowMeans(data[,1:groupsep-1])<1 & rowMeans(data[,groupsep:sampleNum])<1)]=NA

	result <- data.frame(featureData$FeaName, samp1_mean, samp2_mean, featureData$CountMatrix, odd_ratio)#,p)
	colnames( result ) <- c("Feature", group1, group2, featureData$SampName, "Odd-Ratio")#,"p.value")

	result <- result [order(abs(result[,"Odd-Ratio"]),decreasing = TRUE),]

	sig.enriched <- length(result[t(which(result[,"Odd-Ratio"]>1)),1])
	sig.depleted <- length(result[t(which(result[,"Odd-Ratio"]<(-1))),1])
	
	cat("Two Groups of samples test: ", group1, " vs ", group2, "\n")
	cat("Feature number: ", feaNum, "\n")
	cat("Enriched in ",group1," : ", sig.enriched,  "\tDepleted in ",group2," : ",sig.depleted, "\n")
	cat("Result details are saved in : ", resultFileName, "\n")
	write.csv(result,resultFileName)

	return(result)
}

fisherTest <- function(featureData, groupsep, resultFileName = "result.csv", alpha = 0.05, 
group1 = "Group1", group2 = "Group2")
{
	data <- featureData$CountMatrix
	sampleNum=length(featureData$SampName)
	feaNum=length(featureData$FeaName)

	p.value <- rep(NA,times=feaNum)
	s1 <- sum(sum(data[,1:groupsep-1]))
	s2 <- sum(sum(data[,groupsep:sampleNum]))

	for (j in 1:feaNum)
	{
		feature_sample1 <- sum(data[j,1:groupsep-1])#+0.5
		feature_sample2 <- sum(data[j,groupsep:sampleNum])#+0.5

		otherfeature_sample2 <- s2-feature_sample2
		otherfeature_sample1 <- s1-feature_sample1
		
		compare <- matrix(c(feature_sample1,otherfeature_sample1,feature_sample2,otherfeature_sample2),nr=2)
		p.value[j] <- fisher.test(compare)$p.value
	}


	bonferroni.p <- p.adjust(p.value,"bonferroni")
	q.value <- p.adjust(p.value,"fdr")

	result <- data.frame(featureData$FeaName,featureData$CountMatrix,p.value, q.value,bonferroni.p,featureData$Annotation)
	colnames( result ) <- c("Feature", featureData$SampName, "p.value","q.value","bonferroni.p", "Annotation")

	result <- result [order(result[,"p.value"]),]

	sig.p <- length(result[t(which(result[,"p.value"]<alpha)),1])
	sig.q <- length(result[t(which(result[,"q.value"]<alpha)),1])
	sig.bonferroni <- length(result[t(which(result[,"bonferroni.p"]<alpha)),1])

	cat("Two Groups of samples test: ", group1, " vs ", group2, "\n")
	cat("Feature number: ", feaNum, "\n")
	cat("p.value <",alpha," : ",sig.p,"\tq.value <",alpha," : ",sig.q,"\tbonferroni.p <",alpha," : ",sig.bonferroni,"\n")
	cat("Result details are saved in : ", resultFileName, "\n")
	write.csv(result,resultFileName)

	return(result)

}

plotTopVar <- function(compResult, topnum = 10)#, groupseq), isTwoGroups = FALSE)
{

	count <- compResult[2:(dim(compResult)[2]-4)]
	freq <- sweep(count,2,colSums(count),FUN="/")
	tmp <- count[1:topnum,]

	plotdata <- t(tmp)
	windows()
	barplot(plotdata,main="features with top varition",xlab="freq",
beside=TRUE, col=rainbow(dim(freq)[2]),
horiz=FALSE, cex.names=0.6)
	legend("topright",colnames(tmp),fill=rainbow(dim(freq)[2]))
}

plotClust <- function(compResult, alpha=0.05, method="complete")
{
	count <- compResult[, 2:(dim(compResult)[2]-4)]
	freq <- sweep(count,2,colSums(count),FUN="/")

	correlation_feature <- freq
	distance <- as.dist(1-abs(cor(correlation_feature)))
	windows()
	plot(hclust(distance, method=method),main="Samples Clust",cex =2,lwd =3)
}

plotMDS <- function(compResult, alpha=0.05)
{
	count <- compResult[, 2:(dim(compResult)[2]-4)]
	freq <- sweep(count,2,colSums(count),FUN="/")

	correlation_feature <- freq
	distance <- as.dist(1-abs(cor(correlation_feature)))
	fit <- cmdscale(distance, eig=TRUE, k=2)
	x <- fit$points[,1]
	y <- fit$points[,2]
	windows()
	plot(x,y,xlab="Coordinate 1",ylab="Coordinate 2",main="MDS", type="p",col="red")
	text(x,y,labels=colnames(freq),pos=4)
	return(freq)
}

plotHeatMap <- function(featureData, num=600, show_rownames=F)
{
	library(pheatmap)
	freq <- as.matrix(featureData$FreqMatrix)
	tmp <- log2(sweep(freq,1,rowMeans(freq),FUN="/"))
	tmp[which(tmp>1)] = 1
	tmp[which(tmp<(-1))] = -1
	plotnum = min(num,dim(featureData$CountMatrix)[1])
	pheatmap(tmp[1:plotnum,],show_rownames=show_rownames,cellwidth = 18,cellheight = 0.95)
}

KMeans <- function(featureData, clusterNum)
{
	km <- kmeans(t(featureData$CountMatrix),clusterNum)
	cluster <- km$cluster
	center <- km$centers
	cat("Cluster result is", cluster, ".\n")
}

Hcluster <- function(featureData, dist_method = "euclidean", cluster_method = "complete")
{
	d <- dist(t(featureData$CountMatrix), method = dist_method)
	hc <- hclust(d, method = cluster_method)
	plot(hc)
}

PCA <- function (featureData)
{
	dataname = featureData$SampName
	library(stats)
	pr <- prcomp(t(pca_input),cor = TRUE)
	score <- predict(pr)
	windows()
	plot(score[,1:2],main="PCA", type="p")
	text(score[,1],score[,2],labels = dataname[],pos =4)
}

EnvironmentFactor <- function ( featureData, file2, seleFea = 1000, alpha = 0.05 )
{
	sampleNum=length(featureData$SampName)
	Factor <- read.table( file2, header = TRUE, sep = "\t")
	ncols<-dim(Factor)[2]
	nrows<-dim(Factor)[1]
	Factordata<-list()
	Factordata$SampleName<-Factor[1:nrows,1]
	Factordata$FactorName<-colnames(Factor)[-1]
	Factordata$norm <- (Factor[,2:ncols])
	Factordata$norm <- as.matrix(Factordata$norm)
	data <- Factordata$norm
	if( ncols >2 )
	{
	Xcon <- matrix(0,nrows,(ncols-2)*(ncols-1)/2)
	l=1
	for(j in 1:(ncols-2))
	{
		for(k in (j+1):(ncols-1))
		{
			Xcon[,l] <- data[,j]*data[,k]
			l=l+1
		}
	} 
	data <- cbind(data,Xcon)
	}
	for(i in 1:((ncols-1)*ncols/2))
	{
		data[,i] <- scale(data[,i])
	}
	Data <- as.data.frame(data)
	compare_result <- multipleSamplesComp(featureData, isSilent = TRUE)
	compare <-as.matrix(compare_result[1:seleFea,1:sampleNum + 1])
	conclusion_fea <-rep("tmp", seleFea)
	conclusion_fact <- matrix( "tmp", seleFea, ncols*(ncols-1) )
	conclusion_anno <- rep( "tmp", seleFea)
	conclusion_R <- rep( "tmp", seleFea )

	vec<-c(1:(ncols-1))
	factor <- paste("x", vec, sep="")
	if( ncols >2 )
	{fact <- rep(0,(ncols-2)*(ncols-1)/2)
	l=1
	for(j in 1:(ncols-2))
	{
		for(k in (j+1):(ncols-1))
		{
			fact[l]<- paste(factor[j],factor[k],sep="")
			l=l+1
		}
	} 
	factor<-c(factor,fact)
	}
	colnames(Data)=c(factor)
	factor_name <- rep( 0 , ncols*(ncols-1) )
	for( i in 1:(ncols*(ncols-1)/2) )
	{
		factor_name[2*i-1] <- factor[i]
		factor_name[2*i] <- "p-value"
	}
	if(ncols!=2)
	{
		for(i in 1:seleFea)
		{
			cat(i,"\n")
			vec<-c(1:(ncols-1))
			xnam <- paste("x", vec, sep="")
			xcon <- rep(0,(ncols-2)*(ncols-1)/2)
			l=1
			for(j in 1:(ncols-2))
			{
				for(k in (j+1):(ncols-1))
				{
					xcon[l]<- paste(xnam[j],xnam[k],sep="")
					l=l+1
				}
			}			 
			xnam<-c(xnam,xcon)
			fmla <- as.formula(paste("y ~ ", paste(xnam, collapse= "+")))
			y <- compare[i,1:sampleNum]
			lm.new<- lm(fmla,data=Data)
			result=summary(lm.new)
			del_vec=rownames(drop1(lm.new))
			del=(which.min(drop1(lm.new)[2:nrows,4]))+1
			xnam <-setdiff(xnam,del_vec[del])
			while(length(xnam)!=1)
			{
				if(is.nan(sum(result$coef[,4])))
				{
					del_vec=rownames(drop1(lm.new))
					del=(which.min(drop1(lm.new)[2:nrows,4]))+1
					xnam <-setdiff(xnam,del_vec[del])
					fmla <- as.formula(paste("y ~ ", paste(xnam, collapse= "+")))
					lm.new<- lm(fmla,data=Data)
					result=summary(lm.new)
			
				}
				else if(sum(result$coef[,4]>=alpha))
				{
					del_vec=rownames(drop1(lm.new))
					del=(which.min(drop1(lm.new)[2:nrows,4]))+1
					xnam <-setdiff(xnam,del_vec[del])
					fmla <- as.formula(paste("y ~ ", paste(xnam, collapse= "+")))
					lm.new<- lm(fmla,data=Data)
					result=summary(lm.new)
				}
				else
				{
					lm.sol=lm(fmla,data=Data)
					lm.step=step(lm.sol)
					conclusion_fea[i] <- rownames(compare_result)[i]
					conclusion_anno[i] <- as.character(compare_result$Annotation[i])
					conclusion_R[i] <- summary(lm.step)$r.squared
					code_num<-match(rownames(summary(lm.step)$coef)[2:(length(xnam)+1)],factor)
					for(j in 1:length(code_num))
					{
						conclusion_fact[i,] <- 0
						conclusion_fact[i,2*code_num[j]-1] <- summary(lm.step)$coef[(j+1),1]	
						conclusion_fact[i,2*code_num[j]] <- summary(lm.step)$coef[(j+1),4]
					}
					cat(i,rownames(compare_result)[i],"\t")
					cat(rownames(summary(lm.step)$coef)[2:(length(xnam)+1)],"\n")
					cat(summary(lm.step)$coef[2:(length(xnam)+1),1],"\n")
					break
				}
			}
			if(length(xnam)==1)
			{
				fmla <- as.formula(paste("y~",xnam))
				lm.new<- lm(fmla,data=Data)
				result=summary(lm.new)
				if(is.nan(sum(result$coef[,4])))
				{
					conclusion_fea[i] <- rownames(compare_result)[i]
					conclusion_anno[i] <- as.character(compare_result$Annotation[i])
					conclusion_R[i] <- NA
					conclusion_fact[i,] <- NA

					cat(rownames(compare_result)[i],"\tCan't Regression!\n")
				}
				else if(sum(result$coef[,4]>=alpha))
				{
					conclusion_fea[i] <- rownames(compare_result)[i]
					conclusion_anno[i] <- as.character(compare_result$Annotation[i])
					conclusion_R[i] <- NA
					conclusion_fact[i,] <- NA					
					cat(rownames(compare_result)[i],"\tCan't Regression!\n")
				}
				else
				{
					conclusion_fea[i] <- rownames(compare_result)[i]
					conclusion_anno[i] <- as.character(compare_result$Annotation[i])
					conclusion_R[i] <- summary(lm.new)$r.squared
					code_num<-match(rownames(summary(lm.new)$coef)[2:(length(xnam)+1)],factor)
					for(j in 1:length(code_num))
					{
						conclusion_fact[i,] <- 0
						conclusion_fact[i,2*code_num[j]-1] <- summary(lm.new)$coef[(j+1),1]	
						conclusion_fact[i,2*code_num[j]] <- summary(lm.new)$coef[(j+1),4]					
					}

					
					cat(rownames(compare_result)[i],"\t")
					cat(summary(lm.new)$coef[2,1],"\n")
				}
			}
		}
	}
	else
	{
		for(i in 1:seleFea)
		{
			y <- compare[i,1:sampleNum]
			fmla <- as.formula("y~x1")
			lm.sol=lm(fmla,data=Data)
			if(summary(lm.sol)$coef[2,4]<alpha)
			{
				conclusion_fea[i] <- rownames(compare_result)[i]
				conclusion_anno[i] <- as.character(compare_result$Annotation[i])
				conclusion_R[i] <- summary(lm.sol)$r.squared
				code_num<-match(rownames(summary(lm.sol)$coef)[2],factor)
				for(j in 1:length(code_num))
				{
					conclusion_fact[i,] <- 0
					conclusion_fact[i,2*code_num[j]-1] <- summary(lm.sol)$coef[(j+1),1]	
					conclusion_fact[i,2*code_num[j]] <- summary(lm.sol)$coef[(j+1),4]				
				}
				
				cat(rownames(compare_result)[i],"\t")
				cat(summary(lm.sol)$coef[2,1],"\n")
			}
			else
			{
				conclusion_fea[i] <- rownames(compare_result)[i]
				conclusion_anno[i] <- as.character(compare_result$Annotation[i])
				conclusion_R[i] <- NA
				conclusion_fact[i,] <- NA					
				cat(rownames(compare_result)[i],"\tCan't Regression!\n")
			}	
		}
	}
	for( i in 1:seleFea)
	{
		if(conclusion_fea[i]=="tmp")
		{
			conclusion_fea[i]="\t"
		}
		
		if( is.nan(conclusion_anno[i]) )
		{
			if(conclusion_anno[i]=="tmp")
			{
				conclusion_anno[i]="\t"
			}
		}
		
		for(j in 1:(ncols*(ncols-1)/2))
		{
			if( is.nan(conclusion_fact[i,j]) )
			{
				if(conclusion_fact[i,j]=="tmp")
				{
					conclusion_fact[i,j]="\t"
				}
			}
		}
	}
	cat("Result details are saved in : conclusion.csv\n")
	conclusion <- data.frame(conclusion_fea,  conclusion_fact, conclusion_R, conclusion_anno)
	colnames( conclusion ) <- c( "Feature", factor_name, "R-value", "Annotation")
	conclusion <- conclusion [order(conclusion[,"R-value"]),]
	write.csv(conclusion,"conclusion.csv")
}	
	


