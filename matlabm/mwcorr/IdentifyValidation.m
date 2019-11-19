function [allminSQ,allTQ,allIdx] = IdentifyValidation(x,vx,wind,num,rank)
% 移动相关系数法交互验证
% 输入：x，库光谱，pr，库光谱性质，wind，宽度，num，允许用户修改需展示的识别结果个数N，vx，验证集光谱，vy，验证集性质
% 输出：result，性质显示，allminSQ，最小移动相关系数,allTQ，识别参数，
% result：（num+1）*性质：第1行为待识别验证集样品的性质，第2行至num+1行，为识别出来的库样品的性质，其他依次类推
% allminSQ，num*验证集样品数目
% allTQTQ，num*验证集样品数目
	[m,n]=size(vx);
    allminSQ = zeros(num,n);
    allTQ = zeros(num,n);
    allIdx = zeros(num,n);
	[p,w,t] = IdentifyTrain(x,rank);
	for ii=1:n
		y=vx(:,ii);
		[minSQ,TQ,ad] = IdentifyPredictor(x,y,wind,num,p,w,t,1);
		allminSQ(:,ii) = minSQ; 
		allTQ(:,ii) = TQ;
		allIdx(:,ii) = ad;
	end
end