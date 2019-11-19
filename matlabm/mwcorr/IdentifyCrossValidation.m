function [allminSQ,allTQ, allIdx] = IdentifyCrossValidation(x,wind,num,rank)
% 移动相关系数法交互验证
% 输入：x，库光谱，wind，宽度，num，允许用户修改需展示的识别结果个数N，
% 输出：result，性质显示，allminSQ，最小移动相关系数,allTQ，识别参数，
% result：（num+1）*性质：第1行为待识别样品的性质，第2行至num+1行，为识别出来的库样品的性质，其他依次类推
% allminSQ，num*校正集样品数目
% allTQTQ，num*校正集样品数目
	[m,n]=size(x);
    num = min(num,n);
    if num>n-1
        num = n-1;
    end
    allminSQ = zeros(num,n);
    allTQ = zeros(num,n);
    allIdx = zeros(num,n);
	for ii=1:n
		y=x(:,ii);
		xx=x;    
		xx(:,ii)=[];
		[p,w,t] = IdentifyTrain(xx,rank);
		[minSQ,TQ,ad] = IdentifyPredictor(xx,y,wind,num,p,w,t,1);
		ad(ad>=ii) = ad(ad>=ii) + 1;
		allminSQ(:,ii) = minSQ;
		allTQ(:,ii) = TQ;
		allIdx(:,ii) = ad;
	end
end