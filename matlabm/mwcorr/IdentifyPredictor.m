function [minSQ,TQ,ad] = IdentifyPredictor(xx,y,wind,num,p,w,t,topk)
% 移动相关系数法，
%  输入：x，库光谱，pr，库光谱性质，wind，宽度，num，允许用户修改需展示的识别结果个数N，
% 输出：minSQ，最小移动相关系数,TQ，移动相关系统的平均值阈值，ad，最大移动相关系统的平均值阈值对应样品编号
	if size(y,2) > 1
       y = y'; 
    end
	
    [m,n]=size(t);
    if size(xx,2)<20
        x = xx;
        index = [1:size(xx,2)];
        index = index';
    else
        [tun] = ripppcapred(y, p, w );
        tt=t-ones(m,1)*tun;
        sunt=sum(abs(tt'));
        [r2,index]=sort(sunt);
        maxC = 20;
        if n<maxC
            maxC = n;
        end
        x=xx(:,index(1:maxC));
    end
	
	
	[m,n]=size(x);% m，光谱变量数目，n，库光谱数目
    SQ = zeros(m-topk+1,n);
    mTQ = zeros(n);
	for j=1:n
		SQtemp=zeros(n,1); 
		for k=1:m-wind
			xx=y(k:k+wind,1)';
			yy=x(k:k+wind,j)';
			SQtemp(k)=corre(xx,yy); 
		end
		or=ones(1,wind)*corre(xx,yy);
		SQtemp(m-wind+1:m)=or';
		SQtemp=abs(SQtemp);
        
        
        [m2,n2]=sort(SQtemp);
        SQtemp(n2(1:topk-1)) = [];
        SQ(:,j) = SQtemp;
        
        %SQ(:,j) = SQtemp(n2(topk:end));
		oo=SQ(1:m-wind-topk+1,j);
        %mTQ(j)=sum(oo)/(m-wind);% 计算阈值
		 mTQ(j)=sum(oo.*oo)/(m-wind-topk+1);% 计算阈值
	end
	[m1,n1]=sort(mTQ);
    
    
	mSQ=min(SQ);
   
    
    if size(x,2)<num
        num=size(x,2);
    end
    ad = zeros(num,1);
    TQ = zeros(num,1);
    minSQ = zeros(num,1);
    num = min(num,length(n1));
	for i=1:num
		ad(i)=n1(n-i+1);
		TQ(i)=mTQ(ad(i));
		minSQ(i)=mSQ(ad(i));
	end
	ad=(index(ad));
end